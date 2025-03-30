import { useNavigate } from "react-router-dom";

export default function PostCard({ postId, title }) {

  const navigate = useNavigate();

  const handleMoreClick = () => {
    navigate(`/posts/${postId}`);
  };

  return (
    <div
      class="m-6 max-h-48 max-w-[34rem] break-words rounded-lg bg-base p-4 shadow-md shadow-crust hover:bg-surface0"
      onClick={handleMoreClick}
    >
      <div class="h-36">
        <h2 class="mb-2 text-lg font-bold text-lavender">{title}</h2>
        <p class="mb-4 line-clamp-4 overflow-hidden text-ellipsis text-subtext0"></p>
      </div>
    </div>
  );
}
