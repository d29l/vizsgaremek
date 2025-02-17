import { useNavigate } from "react-router-dom";

export default function PostCard({ postId, title, description }) {
  const navigate = useNavigate()
  const handleMoreClick = () => {
    navigate(`/posts/${postId}`);
  };

  return (
    <div class="bg-base max-h-48 shadow-md shadow-crust p-4 rounded-lg break-words m-6">
      <div class="h-36">
        <h2 class="text-lg font-bold mb-2 text-lavender">{title}</h2>
        <p class="text-subtext0 mb-4 overflow-hidden text-ellipsis line-clamp-4">
          {description}
        </p>
      </div>
      <div class="flex justify-end">
        <button onClick={handleMoreClick} class="text-lavender font-semibold hover:underline flex">
          More
        </button>
      </div>
    </div>
  );
}
