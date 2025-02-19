import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";


export default function Navbar() {
  const navigate = useNavigate();

  const [profileClicked, setProfileClicked] = useState(false);

  const handleLogout = () => {
    localStorage.clear();
    navigate('/login');
  };

  const handleProfileClick = () => {
    setProfileClicked(!profileClicked);
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
            className="cursor-pointer mr-4 flex size-10 overflow-hidden rounded-full bg-surface0"
            onClick={handleProfileClick}
          ></div>

          {profileClicked && (
            <div className="absolute right-0 mt-2 w-48 bg-surface0 rounded-lg z-50">
              <div className="p-4">
                <div className="cursor-pointer text-text hover:text-lavender">Profile</div>
                <div onClick={handleLogout} className="cursor-pointer text-text hover:text-lavender">Logout</div>
              </div>
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}