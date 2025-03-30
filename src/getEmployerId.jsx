import axios from "axios";

export const getEmployerId = async (userId) => {
  const response = axios.get(
    `/api/employers/fetchEmployer?UserId=${userId}`,
    {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    },
  );

  const employerId = (await response).data.employerId;
  return employerId;
};
