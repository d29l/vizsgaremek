import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { getUserId } from "../getUserId";
import axios from "axios";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [incorrect, setIncorrect] = useState(false);
  const [showPass, setShowPass] = useState(false);
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const [banner, setBanner] = useState("");
  const [bio, setBio] = useState("");
  const [location, setLocation] = useState("");
  const [profilePicture, setProfilePicture] = useState("");

  const [userId, setUserId] = useState(null);

  const navigate = useNavigate();

  const validateField = (value, fieldName) => {
    if (value === "") {
      return `${fieldName} cannot be empty`;
    }
    return "";
  };

  const handleLogin = async (e) => {
    e.preventDefault();

    const emailErrorMsg = validateField(email, "Email");
    const passwordErrorMsg = validateField(password, "Password");

    setEmailError(emailErrorMsg);
    setPasswordError(passwordErrorMsg);

    if (emailErrorMsg || passwordErrorMsg) return;

    try {
      const response = await axios.post(
        "https://localhost:7077/api/users/loginUser",
        { email, password },
      );

      if (response.status === 200) {
        localStorage.setItem("token", response.data.accessToken);

        const userId_temp = getUserId();
        setUserId(userId_temp);

        console.log("User ID after login:", userId_temp);

        await fetchUserProfile(userId_temp);

        navigate("/");
      }
    } catch (err) {
      if (err.response?.status === 401) {
        setIncorrect(true);
      }
      console.error("Login error:", err);
    }
  };

  const fetchUserProfile = async (userId) => {
    try {
      const response = await axios.get(
        `https://localhost:7077/api/profiles/fetchProfile`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          params: { userId }
        },
      );
    } catch (err) {
      if (err.response?.status === 404) {
        console.error("Profile doesn't exist");
        console.log("Attempting to create profile");
        try {

          const createResponse = await axios.post(
            `https://localhost:7077/api/profiles/createProfile`,
            {
              banner,
              bio: "Bio not filled in yet",
              location: "Location not filled in yet",
              profilePicture
            },
            {
              headers: {
                Authorization: `Bearer ${localStorage.getItem("token")}`,
              },
              params: { userId: userId }
            },
          );
        } catch (err) {
          console.error("Error creating profile: ", err);
        }
      } else {
        console.error("Profile fetch error:", err);
      }
    }
  };

  const handleRedirect = () => {
    navigate("/signup");
  };

  const handleShowPassword = (e) => {
    const checked = e.target.checked;
    setShowPass(checked);
  };

  return (
    <div className="flex min-h-[92vh] items-center justify-center">
      <div className="mx-5 w-[24rem] rounded-lg bg-base py-10 pt-6 shadow-md shadow-crust">
        <div className="flex justify-center">
          <h1 className="text-2xl font-extrabold text-lavender">Sign in</h1>
        </div>
        <div className="flex flex-col items-start px-8 pt-8">
          <form className="flex w-full flex-col" onSubmit={handleLogin}>
            <label className="mb-2 font-bold text-text">Email</label>
            {emailError && (
              <div className="py-1 text-sm font-bold text-red">
                {emailError}
              </div>
            )}
            <input
              placeholder="example@mail.com"
              className="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <label className="mb-2 font-bold text-text">Password</label>
            {passwordError && (
              <div className="py-1 text-sm font-bold text-red">
                {passwordError}
              </div>
            )}
            <input
              type={showPass ? "text" : "password"}
              className="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 focus:border-2 focus:border-lavender focus:outline-none"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <div className="mt-2 flex items-center">
              <input
                type="checkbox"
                className="h-4 w-4 cursor-pointer appearance-none rounded border-2 border-subtext1 transition-colors duration-200 checked:bg-lavender focus:outline-none"
                onChange={handleShowPassword}
              />
              <label className="ml-2 cursor-pointer select-none text-subtext1">
                Show password
              </label>
            </div>

            <div className="mt-6 flex justify-center">
              <button className="w-1/2 rounded-lg bg-lavender p-1 font-bold text-mantle">
                Sign in
              </button>
            </div>

            {incorrect && (
              <div className="self-center pt-4 text-sm font-bold text-red">
                Incorrect email or password
              </div>
            )}

            <div className="mt-6 flex flex-row justify-center">
              <p className="text-subtext1">Don't have an account yet?</p>
              <a
                className="ml-2 cursor-pointer text-lavender hover:underline"
                onClick={handleRedirect}
              >
                Sign up
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
