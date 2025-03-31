import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { getUserId } from '../getUserId';
import api from '../utils/api';
import axios from "axios";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [incorrect, setIncorrect] = useState(false);
  const [showPass, setShowPass] = useState(false);
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const navigate = useNavigate();

  const validateField = (value, fieldName) => {
    if (value === "") {
      return `${fieldName} cannot be empty`;
    }
    return "";
  };

  const fetchUserProfile = async (userId) => {
     if (!userId) {
         console.error("fetchUserProfile: No userId provided.");
         return;
     }
     console.log(`LoginPage: Checking profile for user ID: ${userId}`);
    try {
      const response = await api.get(`/profiles/fetchProfile`, {
        params: { userId },
      });
      console.log("LoginPage: Profile exists.", response.data);

    } catch (err) {
      if (err.response?.status === 404) {
        console.warn(`LoginPage: Profile not found for user ID: ${userId}. Attempting to create.`);
        try {
          const defaultProfileData = {
            bio: "Bio not filled in yet",
            location: "Location not filled in yet",
          };

          const createResponse = await api.post(
            `/profiles/createProfile`,
            defaultProfileData,
            { params: { userId } }
          );
          console.log("LoginPage: Profile created successfully.", createResponse.data);

        } catch (createErr) {
            console.error("LoginPage: Error auto-creating profile:", createErr);
        }

      } else if (err.response?.status === 401) {
          console.error("LoginPage: Unauthorized during profile check (interceptor issue?).");
      } else {
          console.error("LoginPage: Error checking profile:", err);
      }
    }
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
        "/api/users/loginUser",
        { email, password },
      );

      if (
        response.status === 200 &&
        response.data.accessToken &&
        response.data.refreshToken
      ) {
        localStorage.setItem("token", response.data.accessToken);
        localStorage.setItem("refreshToken", response.data.refreshToken);

        const currentUserId = getUserId();

        if(currentUserId) {
            await fetchUserProfile(currentUserId);
        } else {
            console.error("LoginPage: Could not get user ID after login to check profile.");
        }

        navigate("/");

      } else {
        console.error(
          "Login successful but tokens missing in response",
          response.data,
        );
        setIncorrect(true);
      }
    } catch (err) {
      if (err.response?.status === 401) {
        setIncorrect(true);
      } else {
         console.error("Login error:", err);
         setIncorrect(true);
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
              <button type="submit" className="w-1/2 rounded-lg bg-lavender p-1 font-bold text-mantle">
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