import { useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import {
  FaTag,
  FaMapMarkerAlt,
  FaCalendarAlt,
  FaBuilding,
  FaEnvelope,
  FaPhoneAlt,
  FaGlobe,
} from "react-icons/fa";
import Navbar from "../Components/Navbar";
import api from "../utils/api";

export default function PostPage() {
  const { postId } = useParams();
  const [post, setPost] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPost = async () => {
      try {
        setLoading(true);
        const response = await api.get(`/posts/fetchPost/${postId}`);
        setPost(response.data);
      } catch (error) {
        console.error("Error fetching post:", error);
        setPost(null);
      } finally {
        setLoading(false);
      }
    };

    if (postId) {
      fetchPost();
    } else {
      setLoading(false);
    }
  }, [postId]);

  const formatDate = (dateString) => {
    if (!dateString) return "N/A";
    try {
      return new Date(dateString).toLocaleDateString("en-US", {
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    } catch (e) {
      return "Invalid Date";
    }
  };

  if (loading) {
    return (
      <div>
        <Navbar />
        <div className="m-2 flex min-h-[calc(87.5vh+1.5rem)] items-center justify-center p-4">
          <p className="text-subtext0">Loading...</p>
        </div>
      </div>
    );
  }

  if (!post) {
    return (
      <div>
        <Navbar />
        <div className="m-2 flex min-h-[calc(87.5vh+1.5rem)] items-center justify-center p-4">
          <p className="text-red">Post not found or failed to load.</p>
        </div>
      </div>
    );
  }

  const IconWrapper = ({ children }) => (
    <span className="mr-1.5 inline-block align-middle text-subtext1">
      {children}
    </span>
  );

  return (
    <div>
      <Navbar />
      <div className="m-2 grid min-h-[calc(87.5vh+1.5rem)] grid-cols-1 gap-4 p-4 md:grid-cols-3 lg:grid-cols-4">
        <div className="rounded-lg bg-base p-8 shadow-md md:col-span-2 lg:col-span-3">
          <h1 className="mb-2 text-3xl font-bold text-lavender">
            {post.title}
          </h1>
          <p className="mb-1 flex items-center text-sm text-subtext1">
            <IconWrapper>
              <FaTag />
            </IconWrapper>
            <span className="flex-shrink-0 rounded-sm bg-lavender/80 px-2 py-0.5 text-center text-sm font-semibold text-mantle">
              {post.category}
            </span>
          </p>
          <p className="mb-1 text-sm text-subtext1">
            <IconWrapper>
              <FaMapMarkerAlt />
            </IconWrapper>
            <span className="font-medium">Location:</span>{" "}
            <span className="text-text">{post.location}</span>
          </p>
          <p className="mb-4 text-sm text-subtext0">
            <IconWrapper>
              <FaCalendarAlt />
            </IconWrapper>
            <span className="font-medium">Posted:</span>{" "}
            {formatDate(post.createdAt)}
          </p>
          <hr className="my-4 border-surface1" />
          <h2 className="mb-2 text-xl font-semibold text-lavender">
            Job Description
          </h2>
          {post.content &&
            post.content
              .split(".")
              .filter((sentence) => sentence.trim())
              .map((sentence, index) => (
                <p key={index} className="mb-2 text-text">
                  {sentence.trim()}.
                </p>
              ))}
          {(!post.content || !post.content.includes(".")) && (
            <p className="text-text">{post.content}</p>
          )}
        </div>

        {post.employer && (
          <div className="rounded-lg bg-base p-8 shadow-md md:col-span-1 lg:col-span-1">
            <h2 className="mb-3 text-2xl font-semibold text-lavender">
              {post.employer.companyName}
            </h2>
            <hr className="my-3 border-surface1" />
            <p className="mb-2 text-sm text-text">
              <IconWrapper>
                <FaBuilding />
              </IconWrapper>
              <span className="font-medium text-subtext1">Industry:</span>{" "}
              {post.employer.industry}
            </p>
            <p className="mb-2 text-sm text-text">
              <IconWrapper>
                <FaMapMarkerAlt />
              </IconWrapper>
              <span className="font-medium text-subtext1">Address:</span>{" "}
              {post.employer.companyAddress}
            </p>
            <p className="mb-2 text-sm text-text">
              <IconWrapper>
                <FaEnvelope />
              </IconWrapper>
              <span className="font-medium text-subtext1">Email:</span>{" "}
              <a
                href={`mailto:${post.employer.companyEmail}`}
                className="text-lavender hover:underline"
              >
                {post.employer.companyEmail}
              </a>
            </p>
            <p className="mb-2 text-sm text-text">
              <IconWrapper>
                <FaPhoneAlt />
              </IconWrapper>
              <span className="font-medium text-subtext1">Phone:</span>{" "}
              {post.employer.companyPhoneNumber || "N/A"}
            </p>
            {post.employer.companyWebsite && (
              <p className="mb-2 text-sm text-text">
                <IconWrapper>
                  <FaGlobe />
                </IconWrapper>
                <span className="font-medium text-subtext1">Website:</span>{" "}
                <a
                  href={post.employer.companyWebsite}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-lavender hover:underline"
                >
                  {post.employer.companyWebsite}
                </a>
              </p>
            )}
            {post.employer.establishedYear > 0 && (
              <p className="mb-2 text-sm text-text">
                <IconWrapper>
                  <FaCalendarAlt />
                </IconWrapper>
                <span className="font-medium text-subtext1">Established:</span>{" "}
                {post.employer.establishedYear}
              </p>
            )}
            <hr className="my-3 border-surface1" />
            <h3 className="mb-1 text-lg font-semibold text-lavender">
              About Us
            </h3>
            <p className="text-sm text-text">
              {post.employer.companyDescription}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
