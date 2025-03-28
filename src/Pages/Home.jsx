import React, { useEffect, useState } from "react";
import PostCard from "../Components/PostCard";
import Navbar from "../Components/Navbar";
import { fetchPosts } from "../fetchPosts";
import { FaCirclePlus } from "react-icons/fa6";
import axios from "axios";
import { getUserId } from "../getUserId";

export default function Home() {
  const [posts, setPosts] = useState([]);

  const [locationToggled, setLocationToggled] = useState(false);
  const [categoryToggled, setCategoryToggled] = useState(false);

  const [showNewPostPopout, setShowNewPostPopout] = useState(false);

  const handleLocationToggle = () => {
    setLocationToggled(!locationToggled);
  };

  const handleCategoryToggle = () => {
    setCategoryToggled(!categoryToggled);
  };

  async function getPosts() {
    const data = await fetchPosts();
    setPosts(data.data);
  }

  useEffect(() => {}, [posts]);
  useEffect(() => {
    getPosts();
  }, []);

  return (
    <div class="h-screen">
      <Navbar />

      <div class="flex h-[10%] w-full flex-col items-center justify-center">
        <input
          type="text"
          placeholder="Search for a job"
          class="w-1/2 max-w-lg rounded-lg bg-crust p-3 text-text placeholder-subtext0 focus:border-2 focus:border-lavender focus:outline-none"
        />
      </div>

      <div className="flex h-full w-full flex-row overflow-hidden bg-base">
        {/* sidebar */}
        <div className="flex h-screen w-1/6 min-w-[8rem] flex-col bg-base px-3 pt-3 shadow-xl shadow-crust">
          <p className="text-center font-bold text-text">Search Filters</p>
          {/* filter option box */}
          <div class="mt-2 grid h-[4.75rem] w-full grid-cols-2 justify-center rounded-lg bg-mantle pl-5 pt-5 shadow-sm shadow-crust">
            <div class="h-[2.5rem] w-[5.25rem] justify-center">
              <button
                class={`rounded-lg border-2 border-lavender px-2 py-1 font-bold ${locationToggled ? "bg-lavender text-mantle" : "text-lavender"}`}
                onClick={handleLocationToggle}
              >
                Location
              </button>
            </div>

            <div class="h-[2.5rem] w-[5.25rem] justify-center">
              <button
                class={`rounded-lg border-2 border-lavender px-2 py-1 font-bold ${categoryToggled ? "bg-lavender text-mantle" : "text-lavender"}`}
                onClick={handleCategoryToggle}
              >
                Category
              </button>
            </div>
          </div>

          <form class="mt-4">
            {locationToggled && (
              <div class="mt-4">
                <label class="ml-1 font-bold text-text">Location</label>
                <input
                  placeholder="Location"
                  className="mt-1 h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                />
              </div>
            )}

            {categoryToggled && (
              <div class="mt-4">
                <label class="ml-1 font-bold text-text">Category</label>
                <input
                  placeholder="Category"
                  className="mt-1 h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                />
              </div>
            )}
          </form>
        </div>

        <div className="flex w-full flex-col rounded-lg">
          <div class="flex h-[50px] items-center justify-between bg-base">
            <h2 class="ml-2 text-lg font-bold text-text">Jobs</h2>

            <div class="flex flex-row">
              <h2 class="ml-2 text-subtext1">New Post</h2>
              <FaCirclePlus
                class="ml-2 mr-4 cursor-pointer text-2xl text-text hover:text-lavender"
                onClick={() => {
                  setShowNewPostPopout(true);
                }}
              />
            </div>
          </div>

          <div className="h-full overflow-y-auto rounded-xl bg-mantle p-4">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3">
              {posts.map((job) => (
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
      </div>
      {showNewPostPopout && (
        <NewPostPopout
          onClose={() => {
            setShowNewPostPopout(false);
          }}
        />
      )}
    </div>
  );
}

const NewPostPopout = ({ onClose }) => {
  const [title, setTitle] = useState("");
  const [category, setCategory] = useState("");
  const [location, setLocation] = useState("");
  const [content, setContent] = useState("");

  const getEmployerId = async (userId) => {
    try {
      const response = await axios.get("https://localhost:7077/api/employers/fetchEmployer", {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        params: { userId },
      });

      if (response.status === 200) {
        const employerId = await response.data.employerId;
        return employerId;
      }
    } catch (err) {
      console.error("Failed to get employer ID: ", err);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const userId = getUserId();

    const employerId = await getEmployerId(userId);

    try {
      const response = await axios.post(
        "https://localhost:7077/api/posts/newPost",
        {
          title: title,
          content: content,
          category: category,
          location: location,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          params: { EmployerId: employerId, userId: userId },
        },
      );

      if (response.status === 201) {
        window.location.reload();
      }
    } catch (err) {
      console.error("Failed to make post: ", err);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-3xl rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">Create New Post</h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSubmit}>
          <div>
            <label className="mb-2 block text-text">Title</label>
            <input
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Post title"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Category</label>
            <input
              onChange={(e) => setCategory(e.target.value)}
              placeholder="Category"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Location</label>
            <input
              onChange={(e) => setLocation(e.target.value)}
              placeholder="Location"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Content</label>
            <textarea
              onChange={(e) => setContent(e.target.value)}
              placeholder="Post content"
              className="h-[12rem] w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <button
            type="submit"
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
          >
            Post
          </button>
        </form>
      </div>
    </div>
  );
};
