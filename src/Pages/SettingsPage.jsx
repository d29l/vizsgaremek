import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getUserId } from "../getUserId";
import Navbar from "../Components/Navbar";
import axios from "axios";
import { getRole } from "../getRole";

export default function SettingsPage() {
  const [currentPage, setCurrentPage] = useState("Account");

  const tabs = ["Account", "Career"];

  const handlePageSwitch = (tab) => {
    setCurrentPage(tab);
  };

  return (
    <div className="bg-mantle">
      <Navbar />

      <div className="m-4 flex min-h-[87.5vh]">
        <div className="mr-4 flex h-fit w-[12rem] flex-col overflow-hidden rounded-lg bg-base shadow-md shadow-crust">
          {tabs.map((tab) => (
            <div
              key={tab}
              className={`text-md h-[3rem] w-full cursor-pointer items-center justify-center py-3 text-center font-bold text-text hover:bg-surface0 hover:text-lavender ${
                currentPage === tab ? "bg-surface0 text-lavender" : ""
              }`}
              onClick={() => handlePageSwitch(tab)}
            >
              <p>{tab}</p>
            </div>
          ))}
        </div>

        <div className="w-full rounded-lg bg-base pl-1 pt-4 shadow-md shadow-crust">
          <div className="ml-4 flex">
            <h1 className="text-2xl font-extrabold text-lavender">
              {currentPage}
            </h1>
          </div>
          <div className="flex flex-col items-start px-4 pt-8">
            {currentPage === "Account" && <AccountSettings />}
            {currentPage === "Career" && <CareerSettings />}
          </div>
        </div>
      </div>
    </div>
  );
}

const AccountSettings = () => {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");

  const [accountDetailsPopout, setAccountDetailsPopout] = useState(false);
  const [passwordPopout, setPasswordPopout] = useState(false);
  const [deletionPopout, setDeletionPopout] = useState(false);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const response = await axios.get(
          `https://localhost:7077/api/users/fetchUser`,
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          },
        );

        if (response.status === 200) {
          setFirstName(response.data.firstName);
          setLastName(response.data.lastName);
          setEmail(response.data.email);
        }
      } catch (err) {
        console.error("There was an error fetching user data: ", err);
      }
    };
    fetchUser();
  }, []);

  return (
    <div class="flex w-full flex-col">
      {/* Account details */}
      <label className="pb-2 font-bold text-lavender">Account details</label>
      <div class="mb-4 w-full rounded-lg border-[1px] border-surface1 p-5">
        <div class="flex flex-col">
          <div class="flex flex-row items-center">
            <label className="mb-2 font-bold text-text">First Name: </label>
            <label className="mb-2 ml-2 text-subtext0">{firstName}</label>
            {/* <FaEdit class="ml-2 mb-2 text-text hover:text-lavender cursor-pointer"/> */}
          </div>

          <div class="flex flex-row items-center">
            <label className="mb-2 font-bold text-text">Last Name: </label>
            <label className="mb-2 ml-2 text-subtext0">{lastName}</label>
          </div>

          <div class="flex flex-row items-center">
            <label className="mb-2 font-bold text-text">Email: </label>
            <label className="mb-2 ml-2 text-subtext0">{email}</label>
          </div>

          <label
            className="mb-2 cursor-pointer text-sm text-lavender hover:underline"
            onClick={() => setAccountDetailsPopout(true)}
          >
            Edit
          </label>
        </div>
      </div>

      <label className="pb-2 font-bold text-lavender">Danger zone</label>
      <div class="mb-4 flex w-full flex-col space-y-4 rounded-lg border-[1px] border-red p-5">
        <button
          type="submit"
          className="w-[12rem] rounded-lg border-[2px] border-red py-2 font-bold text-red hover:bg-red hover:text-mantle"
          onClick={() => setPasswordPopout(true)}
        >
          Change password
        </button>

        <button
          type="submit"
          className="w-[12rem] rounded-lg border-[2px] border-red py-2 font-bold text-red hover:bg-red hover:text-mantle"
          onClick={() => setDeletionPopout(true)}
        >
          Delete account
        </button>
      </div>

      {accountDetailsPopout && (
        <AccountDetailsPopout
          onClose={() => setAccountDetailsPopout(false)}
          firstName={firstName}
          lastName={lastName}
          email={email}
        />
      )}

      {passwordPopout && (
        <PasswordPopout onClose={() => setPasswordPopout(false)} />
      )}

      {deletionPopout && (
        <AccountDeletionPopout onClose={() => setDeletionPopout(false)} />
      )}
    </div>
  );
};

const CareerSettings = () => {
  const [employerPopout, setEmployerPopout] = useState(false);
  const role = getRole();

  return (
    <div class="flex w-full flex-col">
      {/* Employer request */}
      {role === "Employee" && (
        <div>
          <label className="pb-2 font-bold text-lavender">
            Become an employer
          </label>
          <div class="mb-4 w-full rounded-lg border-[1px] border-surface1 p-5">
            <div class="flex flex-col">
              <label className="mb-2 font-bold text-text">
                Join Our Network of Employers!
              </label>

              <p class="w-1/2 text-subtext1">
                Looking for top talent to grow your team?
                <br />
                Partner with us and connect with skilled professionals ready to
                make an impact.
                <br />
                Post jobs, find candidates, and build your dream workforce
                effortlessly.
              </p>

              <button
                type="submit"
                className="mt-4 w-[12rem] rounded-lg border-[2px] border-lavender py-2 font-bold text-lavender hover:bg-lavender hover:text-mantle"
                onClick={() => {
                  setEmployerPopout(true);
                }}
              >
                Get Started Today
              </button>
            </div>
          </div>
        </div>
      )}

      {employerPopout && (
        <EmployerPopout
          onClose={() => {
            setEmployerPopout(false);
          }}
        />
      )}
    </div>
  );
};

const AccountDetailsPopout = ({ onClose, firstName, lastName, email }) => {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
  });

  useEffect(() => {
    setFormData({
      firstName: firstName || "",
      lastName: lastName || "",
      email: email || "",
    });
  }, [firstName, lastName, email]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSave = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.put(
        `https://localhost:7077/api/users/updateUser`,
        {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        },
      );

      if (response.status === 200) {
        window.location.reload();
      }
    } catch (err) {
      console.error("Failed to update user info: ", err);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-md rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">
          Edit Account Details
        </h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSave}>
          <div>
            <label className="mb-2 block text-text">First Name</label>
            <input
              name="firstName"
              value={formData.firstName}
              onChange={handleInputChange}
              placeholder="Your first name"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Last Name</label>
            <input
              name="lastName"
              value={formData.lastName}
              onChange={handleInputChange}
              placeholder="Your last name"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Email</label>
            <input
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              placeholder="Your email"
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <button
            type="submit"
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
          >
            Save Changes
          </button>
        </form>
      </div>
    </div>
  );
};

const PasswordPopout = ({ onClose }) => {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");

  const [showPass, setShowPass] = useState(false);

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.put(
        `https://localhost:7077/api/users/changePassword`,
        {
          currentPassword,
          newPassword,
          confirmNewPassword,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        },
      );

      if (response.status === 200) {
        window.location.reload();
      }
    } catch (err) {
      console.error("Failed to change password: ", err);
    }
  };

  const handleShowPassword = (e) => {
    const checked = e.target.checked;
    setShowPass(checked);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-md rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">Change Password</h2>

        <form className="flex flex-col space-y-4" onSubmit={handleSave}>
          <div>
            <label className="mb-2 block text-text">Current Password</label>
            <input
              type={showPass ? "text" : "password"}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">New Password</label>
            <input
              type={showPass ? "text" : "password"}
              onChange={(e) => setNewPassword(e.target.value)}
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Confirm New Password</label>
            <input
              type={showPass ? "text" : "password"}
              onChange={(e) => setConfirmNewPassword(e.target.value)}
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
            />
          </div>

          <div>
            <input
              type="checkbox"
              className="h-4 w-4 cursor-pointer appearance-none rounded border-2 border-subtext1 transition-colors duration-200 checked:bg-lavender focus:outline-none"
              onChange={handleShowPassword}
            />
            <label className="ml-2 cursor-pointer select-none text-subtext1">
              Show password
            </label>
          </div>

          <button
            type="submit"
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
          >
            Save Changes
          </button>
        </form>
      </div>
    </div>
  );
};

const AccountDeletionPopout = ({ onClose }) => {
  const [secondStage, setSecondStage] = useState(false);
  const [thirdStage, setThridStage] = useState(false);
  const [password, setPassword] = useState("");

  const navigate = useNavigate();

  const handleDeletion = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.delete(
        `https://localhost:7077/api/users/deleteUser`,
        {
          data: { password },
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (response.status === 200) {
        localStorage.clear();
        navigate("/login");
      }
    } catch (err) {
      console.error("Failed to delete account: ", err);
    }
  };

  const handleStageTwoCancel = () => {
    setSecondStage(false);
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-md rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">Delete Account</h2>

        <form className="flex flex-col space-y-4">
          {!secondStage && !thirdStage && (
            <button
              className="w-full rounded-lg border-[2px] border-red py-2 font-bold text-red hover:bg-red hover:text-mantle"
              onClick={() => setSecondStage(true)}
            >
              Delete account
            </button>
          )}

          {secondStage && (
            <div class="text-center">
              <div className="flex flex-col">
                <label className="text-xl font-bold text-text">
                  Are you sure?
                </label>
                <label className="mb-6 text-sm text-subtext0">
                  This is irreversible
                </label>
              </div>

              <div class="flex flex-row items-center justify-between px-12">
                <button
                  className="w-[8rem] rounded-lg border-[2px] border-red py-2 font-bold text-red hover:bg-red hover:text-mantle"
                  onClick={() => {
                    setSecondStage(false);
                    setThridStage(true);
                  }}
                >
                  Confirm
                </button>
                <button
                  className="w-[8rem] rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
                  onClick={handleStageTwoCancel}
                >
                  Cancel
                </button>
              </div>
            </div>
          )}

          {thirdStage && (
            <div>
              <label className="mb-2 block text-text">Enter password</label>
              <input
                type="password"
                onChange={(e) => setPassword(e.target.value)}
                className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              />
              <button
                type="submit"
                className="w-full rounded-lg mt-4 border-[2px] border-red py-2 font-bold text-red hover:bg-red hover:text-mantle"
                onClick={handleDeletion}
              >
                Delete account
              </button>
            </div>
          )}
        </form>
      </div>
    </div>
  );
};

const EmployerPopout = ({ onClose }) => {
  const [companyName, setCompanyName] = useState("");
  const [companyAddress, setCompanyAddress] = useState("");
  const [companyEmail, setCompanyEmail] = useState("");
  const [companyPhoneNumber, setCompanyPhoneNumber] = useState(0);
  const [industry, setIndustry] = useState("");
  const [companyWebsite, setCompanyWebsite] = useState("");
  const [companyDescription, setCompanyDescription] = useState("");
  const [establishedYear, setEstablishedYear] = useState(0);

  const userId = getUserId();

  const handleRequest = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post(
        "https://localhost:7077/api/employerrequests/postRequest",
        {
          companyName,
          companyAddress,
          companyEmail,
          companyPhoneNumber,
          industry,
          companyWebsite,
          companyDescription,
          establishedYear,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          params: { userId },
        },
      );

      if (response.status === 201) {
        window.location.reload();
      }
    } catch (err) {
      console.error("Failed to make employer request: ", err);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-crust/80">
      <div className="relative mx-5 w-full max-w-4xl rounded-lg bg-base p-6 shadow-xl">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 text-3xl text-lavender"
        >
          ×
        </button>
        <h2 className="mb-4 text-2xl font-bold text-text">
          Become an Employer
        </h2>

        <form className="flex flex-col space-y-4" onSubmit={handleRequest}>
          <div>
            <label className="mb-2 block text-text">Company Name</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyName(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Company Address</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyAddress(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Company Email</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyEmail(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Company Phone Number</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyPhoneNumber(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Industry</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setIndustry(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Company Website</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyWebsite(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Company Description</label>
            <textarea
              className="h-[8rem] w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setCompanyDescription(e.target.value);
              }}
            />
          </div>

          <div>
            <label className="mb-2 block text-text">Established Year</label>
            <input
              className="h-8 w-full rounded-lg bg-mantle px-2 text-subtext0 placeholder-surface2 focus:border-2 focus:border-lavender focus:outline-none"
              onChange={(e) => {
                setEstablishedYear(e.target.value);
              }}
            />
          </div>

          <button
            type="submit"
            className="mt-4 w-full rounded-lg bg-lavender py-2 font-bold text-mantle hover:bg-lavender/90"
          >
            Register Company
          </button>
        </form>
      </div>
    </div>
  );
};
