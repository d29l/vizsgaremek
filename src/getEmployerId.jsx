import api from "./utils/api";

export const getEmployerId = async (userId) => {
  const response = api.get(`/employers/fetchEmployer?UserId=${userId}`);

  const employerId = (await response).data.employerId;
  return employerId;
};
