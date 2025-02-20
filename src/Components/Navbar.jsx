import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getUserId } from "../getUserId";

export default function Navbar() {
  const navigate = useNavigate();

  const [profileClicked, setProfileClicked] = useState(false);

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

  const handleProfileClick = () => {
    setProfileClicked(!profileClicked);
  };

  const handleProfileLink = () => {
    const userId = getUserId();
    navigate(`/profiles/${userId}`)
  };

  return (
    <nav className="h-[8vh] bg-base pb-1">
      <div className="flex h-full items-center justify-between shadow-md shadow-crust">
        <div className="flex items-center space-x-6 pl-4">
          <div className="flex flex-col">
            <h1 className="text-2xl font-extrabold text-lavender">
              Job platform
            </h1>
          </div>

          <div className="flex space-x-4"></div>
        </div>

        <div className="relative">
          <div
            className="mr-4 flex size-10 cursor-pointer overflow-hidden rounded-full bg-surface0"
            onClick={handleProfileClick}
          ></div>

          {profileClicked && (
            <div className="absolute right-0 z-50 mt-2 w-48 rounded-lg bg-surface0">
              <div className="p-4">
                <div
                  onClick={handleProfileLink}
                  className="cursor-pointer text-text hover:text-lavender"
                >
                  Profile
                </div>
                <div
                  onClick={handleLogout}
                  className="cursor-pointer text-text hover:text-lavender"
                >
                  Logout
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}
