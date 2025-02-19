import axios from "axios";
import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import Navbar from "../Components/Navbar";

export default function PostPage() {
  const { userId } = useParams();
  const [userData, setUserData] = useState(null);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [creationDate, setCreationDate] = useState("");

  useEffect(() => {
    const fetchPost = async () => {
      console.log(userId);
      const response = await axios.get(
        `https://localhost:7077/api/users/fetchUser/${userId}`,
      );

      console.log(response.data);
      setFirstName(response.data.firstName);
      setLastName(response.data.lastName);
      setCreationDate(response.data.createdAt);
    };

    fetchPost();
  }, [userId]);

  return (
    <div>
      <Navbar />
      <div class="flex justify-center">
        <div class="m-5 flex min-h-[87.5vh] w-1/2 flex-col items-center rounded-lg bg-base p-8 shadow-lg">
          <div className="size-32 overflow-hidden rounded-full bg-surface0"></div>

          <h1 class="mt-4 text-3xl font-bold text-lavender">
            {firstName} {lastName}
          </h1>
          <p class="text-subtext0">(Employer)</p>
          {/* <div class="bg-mantle w-full h-56"></div> */}
          <p class="text-surface2">Created at: {creationDate}</p>
        </div>
      </div>
    </div>
  );
}
