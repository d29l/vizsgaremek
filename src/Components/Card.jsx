import React from 'react'
import '../Styles/Card.css';

export default function Card() {
  return (
    <div className='cardContainer'>
        <div className='cardBackground'>
            <h3>Job title</h3>
            <p className='description'>Lorem ipsum dolor sit amet consectetur adipisicing elit. Velit iusto eius mollitia itaque molestias pariatur eum adipisci blanditiis! Maxime incidunt quibusdam atque ea quia ducimus magni aliquam possimus culpa adipisci?</p>
            <button>More</button>
        </div>
    </div>
  )
}
