import './Styles/App.css';
import './Styles/theme.css';

import { SearchBar, Card } from './Components';

function App() {
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