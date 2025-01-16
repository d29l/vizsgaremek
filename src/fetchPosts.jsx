import React from 'react'

export const fetchPosts = async () => {
    const response = await fetch('https://localhost:7077/api/posts/fetchPosts')
    const data = await response.json()
    
    


    return data
}