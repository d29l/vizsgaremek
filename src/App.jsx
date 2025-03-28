import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./Pages/Home";
import PostPage from "./Pages/PostPage";
import RegisterPage from "./Pages/RegisterPage";
import LoginPage from "./Pages/LoginPage";
import ProfilePage from "./Pages/ProfilePage";
import CreatePost from "./Pages/CreatePost";
import SettingsPage from "./Pages/SettingsPage";

function App() {
  return (
    <div class="macchiato bg-mantle">
      <div className="min-h-screen overflow-hidden bg-mantle">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/posts/:postId" element={<PostPage />} />
          <Route path="/signup" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/profiles/:userId" element={<ProfilePage />} />
          <Route path="/create-post" element={<CreatePost />} />
          <Route path="/settings" element={<SettingsPage />} />
        </Routes>
      </div>
    </div>
  );
}

export default App;
