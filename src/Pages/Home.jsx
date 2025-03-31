import React, { useEffect, useState } from "react";
import PostCard from "../Components/PostCard";
import Navbar from "../Components/Navbar";
import { FaCirclePlus } from "react-icons/fa6";
import { getUserId } from "../getUserId";
import { getRole } from "../getRole";
import api from "../utils/api";

export default function Home() {
  const [posts, setPosts] = useState([]);
  const [filteredPosts, setFilteredPosts] = useState([]);
  const [locationToggled, setLocationToggled] = useState(false);
  const [categoryToggled, setCategoryToggled] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [locationFilter, setLocationFilter] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [showNewPostPopout, setShowNewPostPopout] = useState(false);
  const [isEmployer, setIsEmployer] = useState(false);

  const checkEmployer = () => {
    const role = getRole();
    if (role === "Employer" || role === "Admin") {
      setIsEmployer(true);
    } else {
      setIsEmployer(false);
    }
  };

  const handleLocationToggle = () => {
    setLocationToggled(!locationToggled);
  };

  const handleCategoryToggle = () => {
    setCategoryToggled(!categoryToggled);
  };

  const getPostData = async () => {
    try {
      const postsResponse = await api.get("/posts/fetchPosts", {});
      if (postsResponse.status === 200) {
        setPosts(postsResponse.data);
      }
    } catch (err) {
      console.error("Failed to fetch posts:", err);
    }
  };

  useEffect(() => {
    getPostData();
    checkEmployer();
  }, []);

  useEffect(() => {
    let currentFilteredPosts = [...posts];

    if (searchQuery) {
      currentFilteredPosts = currentFilteredPosts.filter((post) =>
        post.title.toLowerCase().includes(searchQuery.toLowerCase()),
      );
    }

    if (locationToggled && locationFilter) {
      currentFilteredPosts = currentFilteredPosts.filter(
        (post) =>
          post.location &&
          typeof post.location === "string" &&
          post.location.toLowerCase().includes(locationFilter.toLowerCase()),
      );
    }

    if (categoryToggled && categoryFilter) {
      currentFilteredPosts = currentFilteredPosts.filter(
        (post) =>
          post.category &&
          typeof post.category === "string" &&
          post.category.toLowerCase().includes(categoryFilter.toLowerCase()),
      );
    }

    setFilteredPosts(currentFilteredPosts);
  }, [
    posts,
    searchQuery,
    locationFilter,
    categoryFilter,
    locationToggled,
    categoryToggled,
  ]);

  return (
    <div className="flex h-screen flex-col bg-base">
      <div className="flex-shrink-0">
        <Navbar />
      </div>
      <div className="flex h-[10%] flex-shrink-0 items-center justify-center bg-mantle">
        <input
          type="text"
          placeholder="Search for a job"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="w-1/2 max-w-lg rounded-lg border-2 border-transparent bg-crust p-3 text-text placeholder-subtext0 transition-all duration-200 ease-in-out focus:border-lavender focus:outline-none"
        />
      </div>
      <div className="flex flex-grow flex-row overflow-hidden">
        <div className="flex w-1/6 min-w-[8rem] flex-col overflow-y-auto bg-base px-3 pt-3 shadow-xl shadow-crust">
          <p className="text-center font-bold text-text">Search Filters</p>
          <div className="mt-2 grid h-auto w-full grid-cols-1 justify-center gap-2 rounded-lg bg-mantle p-3 shadow-sm shadow-crust md:grid-cols-2">
            <div className="flex justify-center">
              <button
                className={`h-auto w-full whitespace-normal rounded-lg border-2 border-lavender px-2 py-1 text-xs font-semibold transition-all duration-200 ease-in-out hover:scale-105 active:scale-95 md:px-3 md:py-1.5 md:text-sm ${locationToggled ? "bg-lavender text-mantle hover:bg-opacity-90" : "text-lavender hover:bg-lavender/10"}`}
                onClick={handleLocationToggle}
              >
                Location
              </button>
            </div>
            <div className="flex justify-center">
              <button
                className={`h-auto w-full whitespace-normal rounded-lg border-2 border-lavender px-2 py-1 text-xs font-semibold transition-all duration-200 ease-in-out hover:scale-105 active:scale-95 md:px-3 md:py-1.5 md:text-sm ${categoryToggled ? "bg-lavender text-mantle hover:bg-opacity-90" : "text-lavender hover:bg-lavender/10"}`}
                onClick={handleCategoryToggle}
              >
                Category
              </button>
            </div>
          </div>
          <form className="mt-4 flex-grow">
            {locationToggled && (
              <div className="mb-4 transition-opacity duration-300 ease-in-out">
                <label className="ml-1 font-bold text-text">Location</label>
                <input
                  placeholder="Filter by location"
                  value={locationFilter}
                  onChange={(e) => setLocationFilter(e.target.value)}
                  className="mt-1 h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                />
              </div>
            )}
            {categoryToggled && (
              <div className="mb-4 transition-opacity duration-300 ease-in-out">
                <label className="ml-1 font-bold text-text">Category</label>
                <input
                  placeholder="Filter by category"
                  value={categoryFilter}
                  onChange={(e) => setCategoryFilter(e.target.value)}
                  className="mt-1 h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                />
              </div>
            )}
          </form>
        </div>
        <div className="flex w-5/6 flex-col overflow-hidden">
          <div className="flex h-[50px] flex-shrink-0 items-center justify-between bg-base px-4">
            <h2 className="text-lg font-bold text-text">Jobs</h2>
            {isEmployer && (
              <button
                onClick={() => setShowNewPostPopout(true)}
                className="flex items-center space-x-2 rounded-lg px-3 py-1 text-subtext1 transition-all duration-200 ease-in-out hover:bg-mantle hover:text-lavender focus:outline-none focus:ring-2 focus:ring-lavender/50"
              >
                <span>New Post</span>
                <FaCirclePlus className="cursor-pointer text-2xl text-text transition-transform duration-200 ease-in-out group-hover:rotate-12 group-hover:scale-110 group-hover:text-lavender" />
              </button>
            )}
          </div>
          <div className="flex-grow overflow-y-auto rounded-xl bg-mantle p-4">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3">
              {filteredPosts.length > 0 ? (
                filteredPosts.map((job) => (
                  <div
                    key={job.postId}
                    className="transition-transform duration-200 hover:scale-[1.02]"
                  >
                    <PostCard
                      postId={job.postId}
                      title={job.title}
                      companyName={job.employer?.companyName || "N/A"}
                      location={job.location}
                      category={job.category}
                      description={job.content}
                    />
                  </div>
                ))
              ) : (
                <p className="col-span-full text-center text-subtext0">
                  No jobs match your criteria.
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
      <div
        className={`fixed inset-0 z-40 flex items-center justify-center bg-crust/80 transition-opacity duration-300 ease-out ${
          showNewPostPopout ? "opacity-100" : "pointer-events-none opacity-0"
        }`}
      >
        <div
          onClick={(e) => e.stopPropagation()}
          className={`relative mx-5 w-full max-w-3xl rounded-lg bg-base p-6 shadow-xl transition-all duration-300 ease-out ${
            showNewPostPopout ? "scale-100 opacity-100" : "scale-95 opacity-0"
          }`}
        >
          {showNewPostPopout && (
            <NewPostPopout onClose={() => setShowNewPostPopout(false)} />
          )}
        </div>
      </div>
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
      const response = await api.get("/employers/fetchEmployer", {
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
    if (!userId) {
      console.error("User ID not found");
      return;
    }

    const employerId = await getEmployerId(userId);
    if (!employerId) {
      console.error("Employer ID not found");
      return;
    }

    try {
      const response = await api.post(
        "/posts/newPost",
        {
          title: title,
          content: content,
          category: category,
          location: location,
        },
        {
          params: { EmployerId: employerId, userId: userId },
        },
      );

      if (response.status === 201) {
        onClose();
        window.location.reload();
      }
    } catch (err) {
      console.error("Failed to make post: ", err);
    }
  };

  return (
    <>
      <button
        onClick={onClose}
        className="absolute right-4 top-4 text-3xl text-lavender transition-colors hover:text-opacity-80"
      >
        &times;
      </button>
      <h2 className="mb-4 text-2xl font-bold text-text">Create New Post</h2>
      <form className="flex flex-col space-y-4" onSubmit={handleSubmit}>
        <div>
          <label className="mb-1 block text-sm font-medium text-text">
            Title
          </label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Post title"
            required
            className="h-10 w-full rounded-lg bg-mantle px-3 text-subtext0 placeholder-surface2 transition-colors focus:border-2 focus:border-lavender focus:outline-none"
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-text">
            Category
          </label>
          <input
            type="text"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
            placeholder="Category"
            required
            className="h-10 w-full rounded-lg bg-mantle px-3 text-subtext0 placeholder-surface2 transition-colors focus:border-2 focus:border-lavender focus:outline-none"
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-text">
            Location
          </label>
          <input
            type="text"
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            placeholder="Location"
            required
            className="h-10 w-full rounded-lg bg-mantle px-3 text-subtext0 placeholder-surface2 transition-colors focus:border-2 focus:border-lavender focus:outline-none"
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-text">
            Content
          </label>
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder="Post content"
            required
            rows={6}
            className="w-full rounded-lg bg-mantle px-3 py-2 text-subtext0 placeholder-surface2 transition-colors focus:border-2 focus:border-lavender focus:outline-none"
          />
        </div>
        <button
          type="submit"
          className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle transition-all duration-200 ease-in-out hover:bg-opacity-90 active:scale-95"
        >
          Post
        </button>
      </form>
    </>
  );
};
