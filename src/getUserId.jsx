export const getUserId = () => {
  function decodeToken(token) {
    const [header, payload, signature] = token.split(".");

    const decodedPayload = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(decodedPayload);
  }

  if (localStorage.getItem("token") !== null) {
    const token = localStorage.getItem("token");
    const decodedToken = decodeToken(token);
    if (Date.now() < decodedToken.exp * 1000) {
      return decodedToken.nameid;
    }
  }
};
