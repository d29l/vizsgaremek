import React, { useEffect, useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { getUserId } from "../getUserId";
import api from "../utils/api";

export default function Navbar() {
  const navigate = useNavigate();

  const [profileClicked, setProfileClicked] = useState(false);
  const [profilePicture, setProfilePicture] = useState("");
  const [isLoadingProfile, setIsLoadingProfile] = useState(true);

  const fetchProfile = useCallback(async (id) => {
    if (!id) {
        console.warn("Navbar fetchProfile: No user ID provided.");
        setIsLoadingProfile(false);
        return;
    }
    setIsLoadingProfile(true);
    console.log(`Navbar: Fetching profile for userId: ${id}`);
    try {
      const response = await api.get(`/profiles/fetchProfile`, {
        params: { userId: id },
      });
      setProfilePicture(response.data.profilePicture);
    } catch (err) {
      if (err.response?.status === 404 && err.config?._retry) {
          console.error("Navbar: Profile not found after token refresh.");
      } else if (err.response?.status === 400 && err.config?._retry) {
           console.error("Navbar: Bad request fetching profile after token refresh (invalid userId?).");
      } else if (err.response?.status !== 401) {
          console.error("Navbar: Error fetching profile:", err);
      }
      setProfilePicture("/Storage/Images/default.png");
    } finally {
        setIsLoadingProfile(false);
    }
  }, []);


  useEffect(() => {
    const userId = getUserId();
    if (userId) {
      fetchProfile(userId);
    } else {
      console.warn("Navbar: userId is not available on mount, skipping initial fetch.");
      setIsLoadingProfile(false);
      setProfilePicture("/Storage/Images/default.png");
    }
  }, [fetchProfile]);


  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    navigate("/login");
  };

  const handleSettingsClick = () => {
    setProfileClicked(false);
    navigate("/settings");
  };

  const handleProfileClick = () => {
    setProfileClicked(!profileClicked);
  };

  const handleProfileLink = () => {
    setProfileClicked(false);
    const userId = getUserId();
    if (userId) {
        navigate(`/profiles/${userId}`);
    } else {
        console.error("Navbar: Cannot navigate to profile, user ID not found.");
    }
  };

  const handleHome = () => {
    setProfileClicked(false);
    navigate("/");
  };

  const defaultProfilePic = "/Storage/Images/default.png";

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
            className="mr-4 flex size-10 cursor-pointer items-center justify-center overflow-hidden rounded-full border-2 border-lavender/45 bg-surface0 shadow-md shadow-crust"
            onClick={handleProfileClick}
            role="button"
            aria-haspopup="true"
            aria-expanded={profileClicked}
          >
            {isLoadingProfile ? (
                <div className="h-full w-full animate-pulse bg-surface1"></div>
            ) : (
                <img
                    src={profilePicture || defaultProfilePic}
                    className="h-full w-full object-cover"
                    alt="User profile"
                    onError={(e) => {
                        e.target.onerror = null;
                        e.target.src = defaultProfilePic;
                        console.warn("Navbar: Error loading profile picture, using default.");
                    }}
                 />
            )}
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
