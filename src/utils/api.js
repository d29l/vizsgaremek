import axios from "axios";
import { jwtDecode } from "jwt-decode";

const REFRESH_ENDPOINT = "/api/users/refreshToken";
const LOGIN_ENDPOINT = "/api/users/loginUser";

const api = axios.create({
  baseURL: "/api",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token && config.url !== REFRESH_ENDPOINT && config.url !== LOGIN_ENDPOINT) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);


let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

api.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry && originalRequest.url !== REFRESH_ENDPOINT) {

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
        .then(token => {
          originalRequest.headers['Authorization'] = 'Bearer ' + token;
          return api(originalRequest);
        })
        .catch(err => {
          return Promise.reject(err);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = localStorage.getItem("refreshToken");

      if (!refreshToken) {
        console.error("No refresh token available.");
        isRefreshing = false;
        
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        window.location.href = '/login';
        return Promise.reject(error);
      }

      try {
        console.log("Attempting to refresh token...");
        
        const response = await axios.post(REFRESH_ENDPOINT, { refreshToken });

        const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data;

        if (!newAccessToken || !newRefreshToken) {
           throw new Error("Invalid token refresh response");
        }

        console.log("Token refreshed successfully.");
        
        localStorage.setItem("token", newAccessToken);
        localStorage.setItem("refreshToken", newRefreshToken);

        api.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`;
        originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;

        processQueue(null, newAccessToken);

        return api(originalRequest);

      } catch (refreshError) {
        console.error("Unable to refresh token:", refreshError);

        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        processQueue(refreshError, null);
        window.location.href = '/login';
        isRefreshing = false;
        return Promise.reject(refreshError);
      } finally {
         if (!failedQueue.length) {
             isRefreshing = false;
         }
      }
    }

    return Promise.reject(error);
  }
);

export default api;