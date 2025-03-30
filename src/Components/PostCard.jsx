import { useNavigate } from "react-router-dom";

export default function PostCard({
  postId,
  title,
  companyName,
  location,
  category,
  description,
}) {
  const navigate = useNavigate();

  const handleMoreClick = () => {
    navigate(`/posts/${postId}`);
  };

  return (
    <div
      class="m-6 max-h-48 max-w-[34rem] break-words rounded-lg bg-base p-4 shadow-md shadow-crust hover:bg-surface0"
      onClick={handleMoreClick}
    >
      <div class="flex h-36 flex-col">
        <div class="mb-2 flex items-center justify-between">
          <h2
            class="w-1/2 overflow-hidden text-ellipsis whitespace-nowrap text-lg font-bold text-lavender"
            title={title}
          >
            {title}
          </h2>

          <div class="ml-2 flex-shrink-0 rounded-sm bg-lavender/80 px-2 py-0.5 text-center text-sm font-semibold text-mantle">
            {category}
          </div>
        </div>

        <p class="text-text">{companyName}</p>

        <p class="mb-4 text-sm text-subtext1">{location}</p>

        <p
          class="w-2/3 flex-grow overflow-hidden text-ellipsis whitespace-nowrap text-sm text-subtext1"
          title="Seeking a highly motivated engineer to join our dynamic team, focusing on building scalable cloud solutions using AWS, Kubernetes, and microservices. This is a long description designed to test the line clamping functionality and see how it truncates the text after two lines."
        >
          {description}
        </p>
      </div>
    </div>
  );
}
