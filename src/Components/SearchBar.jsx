import React from 'react'
import '../Styles/App.css'
import '../Styles/SearchBar.css'
import searchIcon from '../Icons/Search.svg'

export default function SearchBar() {
  return (
    <div>
        <div className='container'>
            <div className='searchBackground'>
                <input placeholder='Search for a job' className='searchField' type='text'/>
                <img className='searchIcon' src={searchIcon}></img>
            </div>
        </div>
    </div>
  )
}
