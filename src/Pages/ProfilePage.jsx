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
    const fetchUser = async () => {
      const response = await axios.get(
        `https://localhost:7077/api/users/fetchUser/`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        },
      );

      const date = response.data.createdAt.split("T")[0];
      const [year, month, day] = date.split("-");

      setFirstName(response.data.firstName);
      setLastName(response.data.lastName);
      setCreationDate(`${year} ${month}/${day}`);
      setRole(response.data.role);
    };

    const fetchProfile = async (userId) => {
      const response = await axios.get(
        `https://localhost:7077/api/profiles/fetchProfile?${getUserId}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        },
      );

      setProfilePicture(response.data.profilePicture);
      setBio(response.data.bio);
      setLocation(response.data.location);
    };

    fetchUser();
    fetchProfile();
  }, [userId]);

  return (
    <div>
      <Navbar />
      <div class="flex justify-center">
        <div class="m-5 flex min-h-[87.5vh] w-[52rem] flex-col items-center overflow-hidden rounded-lg bg-base p-8 shadow-lg">
          <div className="relative flex w-full justify-end">
            <FaEdit className="absolute cursor-pointer text-xl text-text hover:text-lavender" onClick={() => setEditPopoutOpen(true)}/>
          </div>

          <div className="size-32 flex items-center overflow-hidden rounded-full bg-surface0">
            <img src={profilePicture}></img>
          </div>

          <h1 class="mt-4 text-3xl font-bold text-lavender">
            {firstName} {lastName}
          </h1>

          <p class="text-subtext1 text-md">{location}</p>
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
    location: "",
    bio: "",
    profilePicture: "",
  });

  useEffect(() => {
    setFormData({
      location: location || "",
      bio: bio || "",
      profilePicture: profilePicture || "",
    });
  }, [location, bio, profilePicture]);

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
      const response = await axios.put(
        `https://localhost:7077/api/profiles/updateProfile?userId=${userId}`,
        {
          headline: "",
          bio: formData.bio,
          location: formData.location,
          profilePicture: formData.profilePicture,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
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
      <div className="relative w-full max-w-md rounded-lg bg-base p-6 shadow-xl mx-5">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">Edit Profile</h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSaveChanges}>
          <div>
            <label className="mb-2 block text-text">Profile picture URL</label>
            <input
              name="profilePicture"
              value={formData.profilePicture}
              onChange={handleInputChange}
              placeholder="Enter image URL"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
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
