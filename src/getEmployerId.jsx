import axios from "axios";

export const getEmployerId = async (userId) => {
  const response = axios.get(
    `https://localhost:7077/api/employers/fetchEmployer?UserId=${userId}`,
    {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    },
  );

  const employerId = (await response).data.employerId;
  return employerId;
};
