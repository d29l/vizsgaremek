import React, { useEffect } from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getUserId } from "../getUserId";
import { getRole } from "../getRole";
import axios from "axios";

export default function Navbar() {
  const navigate = useNavigate();

  const [profileClicked, setProfileClicked] = useState(false);
  const [profilePicture, setProfilePicture] = useState("");

  useEffect(() => {
    const userId = getUserId();
    fetchProfile(userId);
  });

  const fetchProfile = async (userId) => {
    const response = await axios.get(
      `/api/profiles/fetchProfile?`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        params: { userId },
      },
    );

    setProfilePicture(response.data.profilePicture);
  };

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

  const handleSettingsClick = () => {
    navigate("/settings");
  };

  const handleProfileClick = () => {
    setProfileClicked(!profileClicked);
  };

  const handleProfileLink = () => {
    const userId = getUserId();
    navigate(`/profiles/${userId}`);
  };

  const handleHome = () => {
    navigate("/");
  };

  return (
    <nav className="h-[8vh] bg-base pb-1">
      <div className="flex h-full items-center justify-between shadow-md shadow-crust">
        <div className="flex items-center space-x-6 pl-4">
          <div className="flex flex-col">
            <h1
              className="cursor-pointer text-2xl font-extrabold text-lavender"
              onClick={handleHome}
            >
              Job platform
            </h1>
          </div>

          <div className="flex space-x-4"></div>
        </div>

        <div className="relative flex flex-row items-center">
          <div
            className="mr-4 flex size-10 cursor-pointer items-center overflow-hidden rounded-full border-2 border-lavender/45 bg-surface0 shadow-md shadow-crust"
            onClick={handleProfileClick}
          >
            <img src={profilePicture} class="h-full w-full object-cover"></img>
          </div>

          {profileClicked && (
            <div className="absolute right-0 z-50 mt-[9rem] w-48 rounded-lg bg-surface0">
              <div className="p-4">
                <div
                  onClick={handleProfileLink}
                  className="cursor-pointer text-text hover:text-lavender"
                >
                  Profile
                </div>
                <div
                  onClick={handleSettingsClick}
                  className="cursor-pointer text-text hover:text-lavender"
                >
                  Settings
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
