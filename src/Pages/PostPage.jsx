import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import Navbar from "../Components/Navbar";

export default function PostPage() {
  const { postId } = useParams();
  const [post, setPost] = useState(null);

  useEffect(() => {
    const fetchPost = async () => {
      const response = await fetch(
        `https://localhost:7077/api/posts/fetchPost/${postId}`
      );
      const data = await response.json();
      setPost(data);
      console.log(data);
    };

    fetchPost();
  }, [postId]);

  return (
    <div>
      <Navbar/>
      <div class="bg-base shadow-lg rounded-lg p-8 m-5 flex flex-col min-h-[87.5vh]">
        <h1 class="text-2xl font-bold text-lavender mb-4">{post?.title}</h1>

        <div class="text-subtext0 mb-4">
          <span class="font-semibold">Company Name:</span> Company Name
        </div>

        <div class="text-subtext0 mb-4">
          <span class="font-semibold">Company Address:</span> Company Address Line
        </div>

        <div class="text-subtext0 mb-4">
          <span class="font-semibold">Job Description:</span>
          <p class="text-wrap max-w-4xl">{post?.content}</p>
        </div>

        <div class="text-subtext0 mb-4">
          <span class="font-semibold">Posted On:</span> {post?.createdAt}
        </div>

        <div class="flex items-center space-x-2">
          <button class="px-2 py-1 bg-crust text-lavender rounded-md hover:bg-mantle">
            Like
          </button>
          <span class="text-gray-600">{post?.likes}</span>
        </div>
      </div>
    </div>
  );
}
