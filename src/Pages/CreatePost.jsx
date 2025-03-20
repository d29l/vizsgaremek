import { useState } from "react";
import Navbar from "../Components/Navbar";
import { useNavigate } from "react-router-dom";

import axios from "axios";

export default function CreatePost() {
    const handleSubmit = async (e) => {
        e.preventDefault();

        const response = await axios.post(
            `https://localhost:7077/api/posts/newPost/`,
            {
                title,
                content
            },
            {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                },
            }
        );

        handleRedirect();

    }

    const navigate = useNavigate();
    const handleRedirect = () => {
      navigate(`/`);
    };

    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");

    return (
        <div class="bg-mantle">
            <Navbar />

            <div class="flex min-h-[87.5vh] m-4">
                <div class="w-full rounded-lg bg-base py-10 pt-6 shadow-md shadow-crust">
                    <div class="flex ml-4">
                        <h1 class="text-2xl font-extrabold text-lavender">Create Post</h1>
                    </div>
                    <div class="flex flex-col items-start px-4 pt-8">
                        <form class="flex w-full flex-col" onSubmit={handleSubmit}>
                            <label class="mb-2 font-bold text-text">Title</label>
                            <input
                                placeholder="Post title"
                                class="max-w-[24rem] mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                                onChange={(e) => setTitle(e.target.value)}
                            />
                            <label class="mb-2 font-bold text-text">Content</label>
                            <textarea class=" resize-none min-w-[24rem] max-w-[48rem] h-[12rem] mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                                onChange={(e) => setContent(e.target.value)}
                            ></textarea>

                            <div class="mt-6 flex">
                                <button class="min-w-[5rem] rounded-lg bg-lavender p-1 font-bold text-mantle">
                                    Post
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}
