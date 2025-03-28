import axios from "axios";

import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import { getUserId } from "../getUserId";

import Navbar from "../Components/Navbar";
import { FaEdit } from "react-icons/fa";

export default function ProfilePage() {
  const { userId } = useParams();
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [creationDate, setCreationDate] = useState("");
  const [role, setRole] = useState("");
  const [bio, setBio] = useState("");
  const [location, setLocation] = useState("");
  const [profilePicture, setProfilePicture] = useState("");
  const [editPopoutOpen, setEditPopoutOpen] = useState(false);

  useEffect(() => {
    const fetchProfile = async (userId) => {
      try {
        const response = await axios.get(
          `https://localhost:7077/api/profiles/fetchProfile`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
            params: { userId: userId },
          },
        );

        const date = response.data.createdAt.split("T")[0];
        const [year, month, day] = date.split("-");

        setProfilePicture(
          "https://localhost:7077" + response.data.profilePicture,
        );
        setBio(response.data.bio);
        setLocation(response.data.location);
        setFirstName(response.data.firstName);
        setLastName(response.data.lastName);
        setCreationDate(`${year} ${month}/${day}`);
        setRole(response.data.role);
      } catch (err) {
        console.error("There was an error fetching the profile: ", err);
      }
    };

    fetchProfile(userId);
  }, [userId]);

  return (
    <div>
      <Navbar />
      <div class="flex justify-center">
        <div class="m-5 flex min-h-[87.5vh] w-[52rem] flex-col items-center overflow-hidden rounded-lg bg-base p-8 shadow-lg">
          <div className="relative flex w-full justify-end">
            <FaEdit
              className="absolute cursor-pointer text-xl text-text hover:text-lavender"
              onClick={() => setEditPopoutOpen(true)}
            />
          </div>

          <div className="flex size-32 items-center overflow-hidden rounded-full border-2 border-lavender/45 bg-surface0 shadow-md shadow-crust">
            <img src={profilePicture}></img>
          </div>

          <h1 class="mt-4 text-3xl font-bold text-lavender">
            {firstName} {lastName}
          </h1>

          <p class="text-md text-subtext1">{location}</p>
          <p class="text-sm text-subtext0">({role})</p>
          <p class="text-md my-10 w-96 text-center text-text">{bio}</p>
          <p class="text-surface2">Created at: {creationDate}</p>
        </div>
      </div>

      {editPopoutOpen && (
        <ProfileEditPopout
          onClose={() => setEditPopoutOpen(false)}
          profilePicture={profilePicture}
          location={location}
          bio={bio}
          setProfilePicture={setProfilePicture}
          setLocation={setLocation}
          setBio={setBio}
        />
      )}
    </div>
  );
}

const ProfileEditPopout = ({ onClose, location, bio, profilePicture }) => {
  const [formData, setFormData] = useState({
    location: location || "",
    bio: bio || "",
  });
  const [selectedProfilePicture, setSelectedProfilePicture] = useState(null);
  const [selectedBanner, setSelectedBanner] = useState(null);

  const handleFileChange = (e, type) => {
    e.preventDefault();
    const file = e.target.files[0];
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
    const userId = getUserId();

    try {
      const formDataToSend = new FormData();

      formDataToSend.append("Bio", formData.bio);
      formDataToSend.append("Location", formData.location);

      if (selectedProfilePicture) {
        formDataToSend.append("ProfilePicture", selectedProfilePicture);
      }
      if (selectedBanner) {
        formDataToSend.append("Banner", selectedBanner);
      }

      const response = await axios.put(
        "https://localhost:7077/api/profiles/updateProfile",
        formDataToSend,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
            "Content-Type": "multipart/form-data",
          },
        },
      );

      if (response.status === 200) {
        window.location.reload();
      }
    } catch (err) {
      console.error("There was an error updating profile: ", err);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-md rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">Edit Profile</h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSaveChanges}>
          <div>
            <label className="mb-2 block text-text">Banner Image</label>
            <div className="relative flex flex-row items-center">
              <input
                type="file"
                accept="image/*"
                onChange={(e) => handleFileChange(e, "banner")}
                className="absolute inset-0 h-full w-[6rem] cursor-pointer opacity-0"
                id="banner-upload"
              />

              <label
                htmlFor="banner-upload"
                className="flex w-[6rem] min-w-[6rem] cursor-pointer justify-center rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
              >
                Upload
              </label>
              <label className="mb-2 ml-2 mt-1 block text-text">
                {selectedBanner ? `${selectedBanner.name}` : "No file selected"}
              </label>
            </div>
          </div>

          <label className="mb-2 block text-text">Profile Picture</label>
          <div className="relative flex flex-row items-center">
            <input
              type="file"
              accept="image/*"
              onChange={(e) => handleFileChange(e, "profilePicture")}
              className="absolute inset-0 h-full w-[6rem] cursor-pointer opacity-0"
              id="pfp-upload"
            />

            <label
              htmlFor="pfp-upload"
              className="flex w-[6rem] min-w-[6rem] cursor-pointer justify-center rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
            >
              Upload
            </label>
            <label className="mb-2 ml-2 mt-1 block text-text">
              {selectedProfilePicture ? `${selectedProfilePicture.name}` : "No file selected"}
            </label>
          </div>

          <div>
            <label className="mb-2 block text-text">Location</label>
            <input
              name="location"
              value={formData.location}
              onChange={handleInputChange}
              placeholder="Your location"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Bio</label>
            <textarea
              name="bio"
              value={formData.bio}
              onChange={handleInputChange}
              placeholder="Tell us about yourself"
              className="min-h-[100px] w-full rounded-lg bg-mantle p-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <button
            type="submit"
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
          >
            Save Changes
          </button>
        </form>
      </div>
    </div>
  );
};
