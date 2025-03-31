import React, { useState, useEffect } from "react";
import api from "../utils/api";
import { useParams, useNavigate } from "react-router-dom";
import { getUserId } from "../getUserId";

import Navbar from "../Components/Navbar";
import { FaEdit } from "react-icons/fa";

export default function ProfilePage() {
  const { userId } = useParams();
  const loggedInUserId = getUserId();
  const navigate = useNavigate();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [creationDate, setCreationDate] = useState("");
  const [role, setRole] = useState("");
  const [bio, setBio] = useState("");
  const [location, setLocation] = useState("");
  const [profilePicture, setProfilePicture] = useState("");
  const [banner, setBanner] = useState("");
  const [editPopoutOpen, setEditPopoutOpen] = useState(false);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  const isOwnProfile =
    loggedInUserId !== null && loggedInUserId.toString() === userId?.toString();

  useEffect(() => {
    setIsLoading(true);
    setError(null);

    const fetchProfile = async (id) => {
      try {
        console.log(`Fetching profile for userId: ${id}`);
        const response = await api.get(`/profiles/fetchProfile`, {
          params: { userId: id },
        });

        const date = response.data.createdAt.split("T")[0];
        const [year, month, day] = date.split("-");
        setProfilePicture(response.data.profilePicture);
        setBanner(response.data.banner);
        setBio(response.data.bio);
        setLocation(response.data.location);
        setFirstName(response.data.firstName);
        setLastName(response.data.lastName);
        setCreationDate(`${year} ${month}/${day}`);
        setRole(response.data.role);
      } catch (err) {
        console.error("Error fetching profile:", err);
        if (err.response?.status === 404) {
          console.warn(
            `Profile with ID ${id} not found. Redirecting to own profile.`,
          );
          const currentLoggedInUserId = getUserId();
          if (currentLoggedInUserId) {
            navigate(`/profiles/${currentLoggedInUserId}`, { replace: true });
          } else {
            console.error("Cannot redirect: Logged in user ID not found.");
            navigate("/login", { replace: true });
          }
          return;
        } else {
          setError(
            "Failed to load profile information. Please try again later.",
          );
        }
      } finally {
        setIsLoading(false);
      }
    };

    if (userId) {
      fetchProfile(userId);
    } else {
      console.warn("ProfilePage: userId parameter is missing in the URL.");
      setError("No profile ID specified.");
      setIsLoading(false);
    }
  }, [userId, navigate]);

  if (isLoading) {
    return (
      <div>
        <Navbar />
        <div className="flex min-h-[87.5vh] items-center justify-center p-4 text-center text-text">
          Loading profile...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div>
        <Navbar />
        <div className="flex min-h-[87.5vh] items-center justify-center p-4 text-center text-red">
          Error: {error}
        </div>
      </div>
    );
  }

  return (
    <div>
      <Navbar />
      <div className="flex justify-center">
        <div className="m-5 flex min-h-[87.5vh] w-[52rem] flex-col items-center rounded-lg bg-base p-8 shadow-lg">
          <div className="relative mb-[3rem] h-auto w-full">
            <div className="flex h-[14rem] w-full overflow-hidden rounded-lg bg-mantle">
              <img
                className="h-full w-full object-cover"
                src={banner || `/Storage/Banners/default_banner.png`}
                alt="Profile banner"
                onError={(e) => {
                  e.target.onerror = null;
                  e.target.src = `/Storage/Banners/default_banner.png`;
                }}
              />
            </div>
            <div className="absolute left-1/2 flex size-32 -translate-x-1/2 -translate-y-[6rem] items-center overflow-hidden rounded-full border-2 border-lavender/45 bg-surface0 shadow-md shadow-crust">
              <img
                src={profilePicture || `/Storage/Images/default.png`}
                className="h-full w-full object-cover"
                alt="Profile"
                onError={(e) => {
                  e.target.onerror = null;
                  e.target.src = `/Storage/Images/default.png`;
                }}
              />
            </div>
          </div>

          {isOwnProfile && (
            <div className="relative flex w-full justify-end">
              <FaEdit
                className="absolute -top-8 cursor-pointer text-xl text-text hover:text-lavender"
                onClick={() => setEditPopoutOpen(true)}
                aria-label="Edit Profile"
              />
            </div>
          )}

          <h1 className="text-3xl font-bold text-lavender">
            {firstName || "User"} {lastName || ""}
          </h1>
          <p className="text-md text-subtext1">
            {location || "Location unknown"}
          </p>
          <p className="text-sm text-subtext0">
            {role || "Role not specified"}
          </p>
          <p className="text-md my-10 w-96 text-center text-text">
            {bio || "No bio provided."}
          </p>
          <p className="text-surface2">Created at: {creationDate || "N/A"}</p>
        </div>
      </div>

      {isOwnProfile && editPopoutOpen && (
        <ProfileEditPopout
          onClose={() => setEditPopoutOpen(false)}
          currentProfilePicture={profilePicture}
          currentBanner={banner}
          currentLocation={location}
          currentBio={bio}
        />
      )}
    </div>
  );
}

const ProfileEditPopout = ({
  onClose,
  currentLocation,
  currentBio,
  currentProfilePicture,
  currentBanner,
}) => {
  const [formData, setFormData] = useState({
    location: currentLocation || "",
    bio: currentBio || "",
  });
  const [selectedProfilePicture, setSelectedProfilePicture] = useState(null);
  const [selectedBanner, setSelectedBanner] = useState(null);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const handleFileChange = (e, type) => {
    e.preventDefault();
    const file = e.target.files[0];
    if (!file) return;

    if (!file.type.startsWith("image/")) {
      setError(
        `${type === "profilePicture" ? "Profile picture" : "Banner"} must be an image file.`,
      );
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      setError(
        `${type === "profilePicture" ? "Profile picture" : "Banner"} size should not exceed 5MB.`,
      );
      return;
    }

    setError("");
    if (type === "profilePicture") {
      setSelectedProfilePicture(file);
    } else {
      setSelectedBanner(file);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSaveChanges = async (e) => {
    e.preventDefault();
    setError("");
    setIsSaving(true);

    try {
      const formDataToSend = new FormData();

      if (formData.bio !== currentBio) {
        formDataToSend.append("Bio", formData.bio);
      }
      if (formData.location !== currentLocation) {
        formDataToSend.append("Location", formData.location);
      }

      if (selectedProfilePicture) {
        formDataToSend.append("ProfilePicture", selectedProfilePicture);
      }
      if (selectedBanner) {
        formDataToSend.append("Banner", selectedBanner);
      }

      if (
        !formDataToSend.has("Bio") &&
        !formDataToSend.has("Location") &&
        !selectedProfilePicture &&
        !selectedBanner
      ) {
        setError("No changes detected.");
        setIsSaving(false);
        return;
      }

      const response = await api.put(
        "/profiles/updateProfile",
        formDataToSend,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        },
      );

      if (response.status === 200) {
        console.log(
          "Profile updated successfully (response.data):",
          response.data,
        );
        onClose();
        window.location.reload();
      } else {
        setError(`Unexpected response status: ${response.status}`);
      }
    } catch (err) {
      console.error("There was an error updating profile: ", err);
      setError(
        err.response?.data?.message ||
          "Failed to update profile. Please try again.",
      );
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80 backdrop-blur-sm">
      <div className="relative mx-5 w-full max-w-md rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          disabled={isSaving}
          className="absolute right-4 top-4 text-3xl text-subtext1 hover:text-red disabled:opacity-50"
          aria-label="Close edit profile modal"
        >
          &times;
        </button>
        <h2 className="mb-4 text-center text-2xl font-bold text-text">
          Edit Profile
        </h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSaveChanges}>
          <div>
            <label className="mb-1 block text-sm font-medium text-text">
              Banner Image
            </label>
            <div className="relative flex flex-row items-center">
              <input
                type="file"
                accept="image/*"
                onChange={(e) => handleFileChange(e, "banner")}
                className="absolute inset-0 z-10 h-full w-full cursor-pointer opacity-0"
                id="banner-upload"
                disabled={isSaving}
              />
              <label
                htmlFor="banner-upload"
                className={`flex w-[6rem] min-w-[6rem] cursor-pointer justify-center rounded-lg px-3 py-2 text-sm font-bold text-mantle transition-colors ${isSaving ? "cursor-not-allowed bg-surface1" : "bg-lavender hover:bg-lavender/90"}`}
              >
                Upload
              </label>
              <span
                className="ml-3 truncate text-sm text-subtext0"
                title={selectedBanner?.name}
              >
                {selectedBanner ? `${selectedBanner.name}` : "No file selected"}
              </span>
            </div>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-text">
              Profile Picture
            </label>
            <div className="relative flex flex-row items-center">
              <input
                type="file"
                accept="image/*"
                onChange={(e) => handleFileChange(e, "profilePicture")}
                className="absolute inset-0 z-10 h-full w-full cursor-pointer opacity-0"
                id="pfp-upload"
                disabled={isSaving}
              />
              <label
                htmlFor="pfp-upload"
                className={`flex w-[6rem] min-w-[6rem] cursor-pointer justify-center rounded-lg px-3 py-2 text-sm font-bold text-mantle transition-colors ${isSaving ? "cursor-not-allowed bg-surface1" : "bg-lavender hover:bg-lavender/90"}`}
              >
                Upload
              </label>
              <span
                className="ml-3 truncate text-sm text-subtext0"
                title={selectedProfilePicture?.name}
              >
                {selectedProfilePicture
                  ? `${selectedProfilePicture.name}`
                  : "No file selected"}
              </span>
            </div>
          </div>

          <div>
            <label
              htmlFor="location-input"
              className="mb-1 block text-sm font-medium text-text"
            >
              Location
            </label>
            <input
              id="location-input"
              name="location"
              value={formData.location}
              onChange={handleInputChange}
              placeholder="Your location"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-sm text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none disabled:opacity-70"
              disabled={isSaving}
            />
          </div>

          <div>
            <label
              htmlFor="bio-input"
              className="mb-1 block text-sm font-medium text-text"
            >
              Bio
            </label>
            <textarea
              id="bio-input"
              name="bio"
              value={formData.bio}
              onChange={handleInputChange}
              placeholder="Tell us about yourself"
              rows={4}
              className="min-h-[100px] w-full rounded-lg bg-mantle p-2 text-sm text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none disabled:opacity-70"
              disabled={isSaving}
            />
          </div>

          {error && (
            <p className="text-center text-sm font-medium text-red">{error}</p>
          )}

          <button
            type="submit"
            disabled={isSaving}
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle transition-colors hover:bg-lavender/90 disabled:cursor-not-allowed disabled:bg-surface1 disabled:text-subtext1"
          >
            {isSaving ? "Saving..." : "Save Changes"}
          </button>
        </form>
      </div>
    </div>
  );
};
