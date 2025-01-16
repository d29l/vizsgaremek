import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";

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
    <div class="bg-white shadow-lg rounded-lg p-8 m-5 flex flex-col min-h-[87.5vh]">
      <h1 class="text-2xl font-bold text-gray-800 mb-4">{post?.title}</h1>

      <div class="text-gray-600 mb-4">
        <span class="font-semibold">Company Name:</span> Company Name
      </div>

      <div class="text-gray-600 mb-4">
        <span class="font-semibold">Company Address:</span> Company Address Line
      </div>

      <div class="text-gray-600 mb-4">
        <span class="font-semibold">Job Description:</span>
        <p class="text-wrap max-w-4xl">
          {post?.content}
        </p>
      </div>

      <div class="text-gray-600 mb-4">
        <span class="font-semibold">Posted On:</span> {post?.createdAt}
      </div>

      <div class="flex items-center space-x-2">
        <button class="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 focus:ring focus:ring-blue-300">
          Like
        </button>
        <span class="text-gray-600">{post?.likes}</span>
      </div>
    </div>
  );
}
