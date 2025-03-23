import React from 'react'
import axios from 'axios';

export const fetchPosts = async () => {
    const response = await axios.get('https://localhost:7077/api/posts/fetchPosts',
        {
            headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
        }
    )
    const data = await response;
    // console.log(data);

    return data
}