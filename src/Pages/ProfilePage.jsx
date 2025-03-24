import axios from "axios";
import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import Navbar from "../Components/Navbar";
import { getUserId } from "../getUserId";

export default function PostPage() {
  const { userId } = useParams();
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [creationDate, setCreationDate] = useState("");
  const [role, setRole] = useState("");
  const [bio, setBio] = useState("");
  const [location, setLocation] = useState("");
  const [profilePicture, setProfilePicture] = useState("");

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

      setFirstName(response.data.firstName);
      setLastName(response.data.lastName);
      setCreationDate(response.data.createdAt);
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
      setLocation("Miskolc, Hungary");
      setLocation(response.data.location);
    };

    fetchUser();
    fetchProfile();
  }, [userId]);

  return (
    <div>
      <Navbar />
      <div class="flex justify-center">
        <div class="m-5 flex min-h-[87.5vh] w-[52rem] flex-col items-center rounded-lg bg-base p-8 shadow-lg">
          <div className="size-32 overflow-hidden rounded-full bg-surface0">
            <img src={profilePicture}></img>
          </div>

          <h1 class="mt-4 text-3xl font-bold text-lavender">
            {firstName} {lastName}
          </h1>

          <p class="text-subtext2 text-md">{location}</p>
          <p class="text-sm text-subtext0">({role})</p>
          <p class="text-md my-10 w-96 text-center text-text">{bio}</p>
          <p class="text-surface2">Created at: {creationDate}</p>
        </div>
      </div>
    </div>
  );
}
