import { jwtDecode } from "jwt-decode";

export const getUserId = () => {
  const token = localStorage.getItem("token");

  if (!token) {
    return null;
  }

  try {
    const decodedToken = jwtDecode(token);
    const userId = decodedToken.nameid || decodedToken.sub || decodedToken.id;

    if (!userId) {
        console.error("getUserId: User ID claim (nameid, sub, id) not found in decoded token:", decodedToken);
        return null;
    }

    return userId;

  } catch (error) {
    console.error("getUserId: Error decoding token:", error);
    return null;
  }
};
