import './Styles/App.css';
import './Styles/theme.css';

import { SearchBar, Card } from './Components';
import { useState, useEffect } from 'react';

function App() {

  const [database, setDatabase] = useState([])
    console.log(database)
    
  
    useEffect(() => {
        GetFetch()
    }, [])

     function GetFetch(){
      fetch('https://localhost:7077/api/posts/fetchPosts')
      .then(response => response.json())
      .then(data => setDatabase(data))
        
      }


  return (
    <div className="App">
        <SearchBar/>

        <div className='jobsContainer'>
          <Card/>
          <Card/>
          <Card/>
        </div>
    </div>
  );
}

export default App;