import React, { useEffect, useState } from "react";
import SearchBar from "../Components/SearchBar";
import PostCard from "../Components/PostCard";
import { fetchPosts } from "../fetchPosts";
import Navbar from "../Components/Navbar";

export default function Home() {
  const [database, setDatabase] = useState([]);

  async function getPosts() {
    const data = await fetchPosts();
    setDatabase(data.data);
  }

  useEffect(() => { }, [database]);
  return (
    <div>
      <Navbar />
      <div className="w-full bg-mantle shadow-md shadow-crust min-h-[92vh] transition-all duration-200 ease-in-out focus-within:min-h-1 flex flex-col items-center justify-center space-y-4">
        <SearchBar />
        <button class="w-12 h-8 bg-crust rounded-lg text-lavender hover:bg-mantle" onClick={getPosts}>Fasz</button>
      </div>

      <div id="cardsContainer" className="flex h-full flex-col">
        <div class="bg-mantle grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-3 xl:grid-cols-3 gap-y-0">
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
