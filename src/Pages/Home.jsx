import React, { useEffect, useState } from 'react'
import Searchbar from "../Components/Searchbar";
import PostCard from "../Components/PostCard";
import Navbar from '../Components/Navbar';
import { fetchPosts } from '../fetchPosts';

export default function Home() {

    const [database, setDatabase] = useState([])
    async function getPosts() {
        const data = await fetchPosts()
        console.log(data)
            setDatabase(data)
    }



    return (
        <div className="bg-gray-200 overflow-hidden">
            <Navbar />

            <div className="w-full bg-white shadow-md min-h-[92vh] transition-all duration-200 ease-in-out focus-within:min-h-1 flex flex-col items-center justify-center space-y-4">
                <h1 className="font-extrabold text-blue-500 text-xl">Job Platform</h1>
                <Searchbar />
                <button onClick={getPosts}>Fasz</button>
            </div>

            <div className="p-6 h-[78vh]">
                {database.map((job) => (
                    <PostCard postId={job.postId} title={job.title} description={job.content} />
                ))}
            </div>
        </div>
    )
}
