import React, { useEffect, useState } from "react";
import SearchBar from "../Components/SearchBar";
import PostCard from "../Components/PostCard";
import { fetchPosts } from "../fetchPosts";

export default function Home() {
  const [database, setDatabase] = useState([]);

  async function getPosts() {
    const data = await fetchPosts();
    console.log(data);
    setDatabase(data);
  }

  useEffect(() => {}, [database]);

  return (
    <div>
      <div className="w-full bg-white shadow-md min-h-[92vh] transition-all duration-200 ease-in-out focus-within:min-h-1 flex flex-col items-center justify-center space-y-4">
        <h1 className="font-extrabold text-blue-500 text-xl">Job Platform</h1>
        <SearchBar />
        <button onClick={getPosts}>Fasz</button>
      </div>

      <div id="cardsContainer" className="flex h-full flex-col">
        <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-3 xl:grid-cols-3 gap-y-0">
          {database.map((job) => (
            <PostCard
              key={job.postId}
              postId={job.postId}
              title={job.title}
              description={job.content}
            />
          ))}
        </div>
      </div>
    </div>
  );
}
