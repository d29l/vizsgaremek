import React from 'react'
import '../Styles/App.css'
import '../Styles/SearchBar.css'
import searchIcon from '../Icons/Search.svg'

export default function SearchBar() {
  return (
    <div>
        <div className='searchContainer'>
            <div className='searchBackground'>
                <input placeholder='Search for a job' className='searchField' type='text'/>
                <img className='searchIcon' src={searchIcon} alt='search icon'></img>
            </div>
        </div>
    </div>
  )
}
