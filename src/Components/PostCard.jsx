export default function PostCard({ postId, title, description }) {
    return (
      <div class="bg-white max-h-48 border border-gray-300 shadow-md p-4 rounded-lg break-words">
        <div class="h-36">
          <h2 class="text-lg font-bold mb-2">{title}</h2>
          <p class="text-gray-600 mb-4 overflow-hidden text-ellipsis line-clamp-4">
            {description}
          </p>
        </div>
        <div class="flex justify-end">
          <button class="text-blue-500 font-semibold hover:underline flex">
            More
          </button>
        </div>
      </div>
    );
  }