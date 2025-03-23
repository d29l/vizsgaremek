import { useNavigate } from "react-router-dom";
import { useState } from "react";
import axios from "axios";

export default function RegisterPage() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [showPass, setShowPass] = useState(false);

  const [firstNameError, setFirstNameError] = useState(false);
  const [lastNameError, setLastNameError] = useState(false);
  const [emailError, setEmailError] = useState(false);
  const [passwordError, setPasswordError] = useState(false);
  const [confirmPasswordError, setConfirmPasswordError] = useState(false);

  const navigate = useNavigate();

  const validateField = (value, fieldName, minLength = 2) => {
    if (value === "") {
      return `${fieldName} cannot be empty`;
    }
    if (value.length < minLength) {
      return `${fieldName} must be at least ${minLength} characters`;
    }
    return false;
  };

  const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      return "Email is not in a valid format";
    }
    return false;
  };

  const validatePassword = (password) => {
    if (password.length < 8) {
      return "Password must be at least 8 characters long";
    }
    if (!/[A-Z]/.test(password)) {
      return "Password must contain at least one capital letter";
    }
    if (!/\d/.test(password)) {
      return "Password must contain at least one number";
    }
    return false;
  };

  const validateConfirmPassword = (password, confirmPassword) => {
    if (password !== confirmPassword) {
      return "Passwords do not match";
    }
    return false;
  };

  const checkInputData = () => {
    const firstNameErrorMsg = validateField(firstName, "First name");
    const lastNameErrorMsg = validateField(lastName, "Last name");
    const emailErrorMsg = validateEmail(email);
    const passwordErrorMsg = validatePassword(password);
    const confirmPasswordErrorMsg = validateConfirmPassword(
      password,
      confirmPassword,
    );

    setFirstNameError(firstNameErrorMsg);
    setLastNameError(lastNameErrorMsg);
    setEmailError(emailErrorMsg);
    setPasswordError(passwordErrorMsg);
    setConfirmPasswordError(confirmPasswordErrorMsg);

    if (
      !firstNameErrorMsg &&
      !lastNameErrorMsg &&
      !emailErrorMsg &&
      !passwordErrorMsg &&
      !confirmPasswordErrorMsg
    ) {
      return true;
    } else {
      return false;
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();

    const isFormValid = checkInputData();

    if (!isFormValid) {
      return;
    }

    try {
      const response = await axios.post(
        "https://localhost:7077/api/users/registerUser",
        {
          firstName,
          lastName,
          email,
          password,
        },
      );

      if (response.status === 201) {
        navigate("/login");
      } else {
        console.log(response.data.message || "Registration failed");
      }
    } catch (err) {
      console.log(
        err.response?.data?.message || "An error occurred during registration",
      );
    }
  };

  const handleRedirect = () => {
    navigate("/login");
  };

  const handleShowPassword = (e) => {
    const checked = e.target.checked;
    setShowPass(checked);
  };

  return (
    <div className="flex min-h-[92vh] items-center justify-center">
      <div className="w-[24rem] rounded-lg bg-base py-10 pt-6 shadow-md shadow-crust">
        <div className="flex justify-center">
          <h1 className="text-2xl font-extrabold text-lavender">Sign up</h1>
        </div>
        <div className="flex flex-col items-start px-8 pt-8">
          <form className="flex w-full flex-col" onSubmit={handleRegister}>
            <div className="flex flex-row">
              <div className="flex w-1/2 flex-col">
                <label className="mb-2 font-bold text-text">First Name</label>
                <input
                  placeholder="John"
                  className="mb-2 mr-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                />
                {firstNameError && (
                  <div className="self-center pt-1 text-sm font-bold text-red">
                    {firstNameError}
                  </div>
                )}
              </div>

              <div className="flex w-1/2 flex-col">
                <label className="mb-2 ml-2 font-bold text-text">
                  Last Name
                </label>
                <input
                  placeholder="Doe"
                  className="mb-2 ml-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                />
                {lastNameError && (
                  <div className="self-center pl-2 pt-1 text-sm font-bold text-red">
                    {lastNameError}
                  </div>
                )}
              </div>
            </div>

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

            <label className="mb-2 font-bold text-text">Confirm password</label>
            <input
              type={showPass ? "text" : "password"}
              className="mb-2 h-8 rounded-lg bg-mantle pl-2 text-subtext0 focus:border-2 focus:border-lavender focus:outline-none"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
            />
            {confirmPasswordError && (
              <div className="self-center pt-1 text-sm font-bold text-red">
                {confirmPasswordError}
              </div>
            )}

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
              <button
                type="submit"
                className="w-1/2 rounded-lg bg-lavender p-1 font-bold text-mantle"
              >
                Sign Up
              </button>
            </div>

            <div className="mt-6 flex flex-row justify-center">
              <p className="text-subtext1">Already have an account?</p>
              <a
                className="ml-2 cursor-pointer text-lavender hover:underline"
                onClick={handleRedirect}
              >
                Sign in
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
