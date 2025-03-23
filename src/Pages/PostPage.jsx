import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import Navbar from "../Components/Navbar";
import axios from "axios";

export default function PostPage() {
  const { postId } = useParams();
  const [post, setPost] = useState(null);

  useEffect(() => {
    const fetchPost = async () => {
      const response = await axios.get(
        `https://localhost:7077/api/posts/fetchPost/${postId}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        },
      );
      const data = await response;
      setPost(data);
      console.log(data);
    };

    fetchPost();
  }, [postId]);

  return (
    <div>
      <Navbar />
      <div class="m-5 flex min-h-[87.5vh] flex-col rounded-lg bg-base p-8 shadow-lg">
        <h1 class="mb-4 text-2xl font-bold text-lavender">{post?.title}</h1>

        <div class="mb-4 text-subtext0">
          <span class="font-semibold">Company Name:</span> Company Name
        </div>

        <div class="mb-4 text-subtext0">
          <span class="font-semibold">Company Address:</span> Company Address
          Line
        </div>

        <div class="mb-4 text-subtext0">
          <span class="font-semibold">Job Description:</span>
          <p class="max-w-4xl text-wrap">{post?.content}</p>
        </div>

        <div class="mb-4 text-subtext0">
          <span class="font-semibold">Posted On:</span> {post?.createdAt}
        </div>

        <div class="flex items-center space-x-2">
          <button class="rounded-md bg-crust px-2 py-1 text-lavender hover:bg-mantle">
            Like
          </button>
          <span class="text-gray-600">{post?.likes}</span>
        </div>
      </div>
    </div>
  );
}
