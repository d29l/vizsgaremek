import React from 'react'

export async function fetchPosts() {
    const response = await fetch('https://localhost:7077/api/posts/fetchPosts')
    const data = await response.json()
    
    


    return data
}