import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./Pages/Home";
import PostPage from "./Pages/PostPage";
import Navbar from "./Components/Navbar";
import RegisterPage from "./Pages/RegisterPage";
import LoginPage from "./Pages/LoginPage";
import ProfilePage from "./Pages/ProfilePage";
import CreatePost from "./Pages/CreatePost";

function App() {
  return (
    <div class="frappe">
      <div className="bg-mantle overflow-hidden min-h-screen">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/posts/:postId" element={<PostPage />} />
          <Route path="/signup" element={<RegisterPage/>} />
          <Route path="/login" element={<LoginPage/>} />
          <Route path="/profiles/:userId" element={<ProfilePage/>} />
          <Route path="/create-post" element={<CreatePost/>} />
        </Routes>
      </div>
    </div>
  );
}

export default App;
