import { useState } from "react";
import Navbar from "../Components/Navbar";
import axios from "axios";
import { getUserId } from "../getUserId";

export default function SettingsPage() {
  const [currentPage, setCurrentPage] = useState("Account");

  const tabs = ["Account"];

  const handlePageSwitch = (tab) => {
    setCurrentPage(tab);
  };

  return (
    <div className="bg-mantle">
      <Navbar />

      <div className="m-4 flex min-h-[87.5vh]">
        <div className="mr-4 flex h-fit w-[12rem] flex-col overflow-hidden rounded-lg bg-base shadow-md shadow-crust">
          {tabs.map((tab) => (
            <div
              key={tab}
              className={`text-md h-[3rem] w-full cursor-pointer items-center justify-center py-3 text-center font-bold text-text hover:bg-surface0 hover:text-lavender ${
                currentPage === tab ? "bg-surface0 text-lavender" : ""
              }`}
              onClick={() => handlePageSwitch(tab)}
            >
              <p>{tab}</p>
            </div>
          ))}
        </div>

        <div className="w-full rounded-lg bg-base pl-1 pt-4 shadow-md shadow-crust">
          <div className="ml-4 flex">
            <h1 className="text-2xl font-extrabold text-lavender">
              {currentPage}
            </h1>
          </div>
          <div className="flex flex-col items-start px-4 pt-8">
            {currentPage === "Account" && <AccountSettings />}
          </div>
        </div>
      </div>
    </div>
  );
}

const AccountSettings = () => {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");

  const saveAccountDetails = (e) => {
    const userId = getUserId();
    e.preventDefault();

    const response = axios.put(
      `https://localhost:7077/api/users/updateUser/${userId}`,
      {
        firstName,
        lastName,
        email,
      },
      {
        headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
    }
    );
  };
  return (
    <div class="flex w-full flex-col">
      {/* Account details */}
      <label className="pb-2 font-bold text-lavender">Account details</label>
      <div class="mb-4 w-full rounded-lg border-[1px] border-surface1 p-5">
        <form class="flex flex-col" onSubmit={saveAccountDetails}>
          <label className="mb-2 text-text">First Name</label>
          <input
            placeholder="John"
            className="mb-2 mr-2 h-8 max-w-[8rem] rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          <label className="mb-2 text-text">Last Name</label>
          <input
            placeholder="John"
            className="mb-2 mr-2 h-8 max-w-[8rem] rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
          />

          <label className="mb-2 text-text">Email</label>
          <input
            placeholder="example@mail.com"
            className="mb-2 h-8 max-w-[16rem] rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          {/* <a className="cursor-pointer text-lavender hover:underline">
            Change password
          </a> */}

          <button class="mt-4 min-w-[5rem] max-w-[3rem] rounded-lg bg-lavender p-1 font-bold text-mantle">
            Save
          </button>
        </form>
      </div>
    </div>
  );
};
