-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 31. 19:39
-- Kiszolgáló verziója: 10.4.25-MariaDB
-- PHP verzió: 8.1.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `projekt`
--
CREATE DATABASE IF NOT EXISTS `projekt` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE `projekt`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `connections`
--

CREATE TABLE `connections` (
  `ConnectionID` int(11) NOT NULL,
  `RequesterID` int(11) NOT NULL,
  `ReceiverID` int(11) NOT NULL,
  `Status` enum('Pending','Accepted','Declined') COLLATE utf8_hungarian_ci DEFAULT 'Pending',
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `employerrequest`
--

CREATE TABLE `employerrequest` (
  `ApplicantID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `CompanyName` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyAddress` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyEmail` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyPhoneNumber` int(11) NOT NULL,
  `Industry` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyWebsite` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyDescription` text COLLATE utf8_hungarian_ci NOT NULL,
  `EstabilishedYear` year(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `employers`
--

CREATE TABLE `employers` (
  `EmployerID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `CompanyName` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyAddress` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `CompanyEmail` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `CompanyPhoneNumber` int(11) NOT NULL,
  `Industry` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `CompanyWebsite` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `CompanyDescription` text COLLATE utf8_hungarian_ci DEFAULT NULL,
  `EstablishedYear` year(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `employers`
--

INSERT INTO `employers` (`EmployerID`, `UserID`, `CompanyName`, `CompanyAddress`, `CompanyEmail`, `CompanyPhoneNumber`, `Industry`, `CompanyWebsite`, `CompanyDescription`, `EstablishedYear`) VALUES
(14, 29, 'Acme Corporation', '123 Innovation Drive, San Jose, CA 95112', 'info@acmecorp.com', 5551234, 'Technology', 'www.acmecorp.com', 'A leading provider of cutting-edge technology solutions.', 0000),
(15, 30, 'Global Industries', '456 World Trade Center, New York, NY 10007', 'contact@globalind.net', 5555678, 'Manufacturing', 'www.globalindustries.com', 'A global leader in manufacturing and distribution.', 1950),
(16, 31, 'Innovate Solutions', '789 Tech Park, Berlin, 10117, Germany', 'hello@innovatesol.de', 49301234, 'Consulting', 'www.innovatesol.com', 'A consulting firm specializing in innovative business strategies.', 2005),
(17, 32, 'Tech Forward', '101 Silicon Avenue, Palo Alto, CA 94303', 'hr@techfwd.com', 5559012, 'Software Development', 'www.techfwd.io', 'A dynamic software development company focused on the future.', 2010),
(18, 33, 'Bright Horizons', '22 Market Street, Manchester, M1 1PT, UK', 'enquiries@brighthorizons.co.uk', 44161234, 'Education', 'www.brighthorizons.co.uk', 'A leading provider of education and childcare services.', 1988);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `messages`
--

CREATE TABLE `messages` (
  `MessageID` int(11) NOT NULL,
  `SenderID` int(11) NOT NULL,
  `ReceiverID` int(11) NOT NULL,
  `Content` text COLLATE utf8_hungarian_ci NOT NULL,
  `SentAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `posts`
--

CREATE TABLE `posts` (
  `PostID` int(11) NOT NULL,
  `EmployerID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `Title` text COLLATE utf8_hungarian_ci NOT NULL,
  `Category` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `Location` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `Content` text COLLATE utf8_hungarian_ci NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `posts`
--

INSERT INTO `posts` (`PostID`, `EmployerID`, `UserID`, `Title`, `Category`, `Location`, `Content`, `CreatedAt`) VALUES
(16, 14, 29, 'Software Engineer - Full Stack', 'Software Development', 'San Jose, CA', 'Join our team at Acme Corporation and work on cutting-edge web applications. We are looking for experienced full-stack developers with a passion for innovation.', '2025-03-31 19:34:56'),
(17, 14, 29, 'Senior Project Manager', 'Project Management', 'San Jose, CA', 'Acme Corporation is seeking a senior project manager to lead complex technology projects. Must have experience with Agile methodologies and a proven track record of successful project delivery.', '2025-03-31 19:35:03'),
(18, 15, 30, 'Manufacturing Engineer', 'Manufacturing', 'New York, NY', 'Global Industries is hiring a manufacturing engineer to optimize production processes and improve efficiency. Experience in a fast-paced manufacturing environment is essential.', '2025-03-31 19:35:16'),
(19, 15, 30, 'Supply Chain Analyst', 'Supply Chain', 'New York, NY', 'Join our team as a supply chain analyst and help us streamline our global supply chain operations. Strong analytical and problem-solving skills are required.', '2025-03-31 19:35:22'),
(20, 16, 31, 'Business Strategy Consultant', 'Consulting', 'Berlin, Germany', 'Innovate Solutions is looking for a business strategy consultant to work with clients on developing and implementing innovative business strategies. Strong analytical and communication skills are a must.', '2025-03-31 19:35:34'),
(21, 16, 31, 'Data Analyst (Junior)', 'Data Analysis', 'Berlin, Germany', 'Join our dynamic team at Innovate Solutions, as a junior data analyst, and help our clients to make informed, data driven decisions.', '2025-03-31 19:35:41'),
(22, 17, 32, 'Front-End Developer (React)', 'Front-End Development', 'Palo Alto, CA', 'Tech Forward is seeking a skilled front-end developer with expertise in React. Join us and work on building modern and user-friendly web interfaces.', '2025-03-31 19:35:53'),
(23, 17, 32, 'DevOps Engineer', 'DevOps', 'Palo Alto, CA', 'We are looking for a DevOps engineer to help us automate our deployment processes and improve our infrastructure. Experience with AWS and Docker is highly desirable.', '2025-03-31 19:36:00'),
(24, 18, 33, 'Early Childhood Educator', 'Education', 'Manchester, UK', 'Bright Horizons is hiring passionate early childhood educators to join our team. Help us create a nurturing and stimulating learning environment for young children.', '2025-03-31 19:36:11'),
(25, 18, 33, 'Education Administrator', 'Education Administration', 'Manchester, UK', 'We are seeking an organized and detail-oriented education administrator to manage our administrative tasks. Experience in an educational setting is preferred.', '2025-03-31 19:36:17');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `profiles`
--

CREATE TABLE `profiles` (
  `ProfileID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  `Banner` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `Bio` text COLLATE utf8_hungarian_ci DEFAULT NULL,
  `Location` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL,
  `ProfilePicture` varchar(255) COLLATE utf8_hungarian_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `profiles`
--

INSERT INTO `profiles` (`ProfileID`, `UserID`, `Banner`, `Bio`, `Location`, `ProfilePicture`) VALUES
(7, 19, '/Storage/Banners/default_banner.png', 'Software developer passionate about clean code.', 'San Francisco, CA', '/Storage/Images/default.png'),
(8, 20, '/Storage/Banners/default_banner.png', 'Marketing professional with a focus on digital strategy.', 'New York, NY', '/Storage/Images/default.png'),
(9, 21, '/Storage/Banners/default_banner.png', 'Data scientist exploring the world of AI.', 'Seattle, WA', '/Storage/Images/default.png'),
(10, 22, '/Storage/Banners/default_banner.png', 'Graphic designer with a love for minimalist design.', 'Austin, TX', '/Storage/Images/default.png'),
(11, 23, '/Storage/Banners/default_banner.png', 'Project manager specializing in agile methodologies.', 'Chicago, IL', '/Storage/Images/default.png'),
(12, 24, '/Storage/Banners/default_banner.png', 'Financial analyst with expertise in market trends.', 'Boston, MA', '/Storage/Images/default.png'),
(13, 25, '/Storage/Banners/default_banner.png', 'Content writer creating engaging stories.', 'Portland, OR', '/Storage/Images/default.png'),
(14, 26, '/Storage/Banners/default_banner.png', 'HR specialist focusing on employee satisfaction.', 'Denver, CO', '/Storage/Images/default.png'),
(15, 27, '/Storage/Banners/default_banner.png', 'Sales representative with a strong customer focus.', 'Miami, FL', '/Storage/Images/default.png'),
(16, 28, '/Storage/Banners/default_banner.png', 'Systems administrator maintaining network infrastructure.', 'Los Angeles, CA', '/Storage/Images/default.png'),
(17, 29, '/Storage/Banners/default_banner.png', 'Acme Corporation: Leading the way in innovative solutions.', 'Headquarters: San Jose, CA', '/Storage/Images/default.png'),
(18, 30, '/Storage/Banners/default_banner.png', 'Global Industries: Connecting the world through technology.', 'Global presence: New York, London, Tokyo', '/Storage/Images/default.png'),
(19, 31, '/Storage/Banners/default_banner.png', 'Innovate Solutions: Transforming ideas into reality.', 'Innovation Hub: Berlin, Germany', '/Storage/Images/default.png'),
(20, 32, '/Storage/Banners/default_banner.png', 'Tech Forward: Building the future, one line of code at a time.', 'Silicon Valley: Palo Alto, CA', '/Storage/Images/default.png'),
(21, 33, '/Storage/Banners/default_banner.png', 'Bright Horizons: Creating opportunities, inspiring growth.', 'European Office: Manchester, UK', '/Storage/Images/default.png');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `FirstName` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `LastName` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `Email` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `Password` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `RefreshToken` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `Role` enum('Employee','Employer','Admin') COLLATE utf8_hungarian_ci DEFAULT 'Employee',
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `IsVerified` tinyint(1) NOT NULL DEFAULT 0,
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`UserID`, `FirstName`, `LastName`, `Email`, `Password`, `RefreshToken`, `Role`, `CreatedAt`, `IsVerified`, `IsActive`) VALUES
(18, 'System', 'Admin', 'systemadmin@jobplatform.hu', 'a45s8WxvOFlh3xjrlWwt7TLF7Ew4cKjqUkEFvk0fnf/trQ7AZOTfLqgatG9bEnbn', 'kkwBucT2gKIKMeYkY28E9aqM/AADTlNQpucYnwnJ8c7HOrivYsaJ7v4N2JLcn+QVBX1vC5tXpWiqLirV5ZG+7g==', 'Admin', '2025-03-31 18:10:56', 1, 1),
(19, 'Alice', 'Johnson', 'alice.johnson@example.com', 'lK9myKOTERJU9naeIa69zHOx2TXubX5Ak8rsKd4aRubPonFQ/HKolzJH0IgVWdJA', '', 'Employee', '2025-03-31 18:48:29', 0, 1),
(20, 'Bob', 'Smith', 'bob.smith@example.com', 'ADtFCOjMgE95gr1WXhCfYvnx9gZNjC3TGX4FJpXcbrINP8sCeiOCW1pwD3uhQrzU', '', 'Employee', '2025-03-31 18:48:38', 0, 1),
(21, 'Charlie', 'Williams', 'charlie.williams@example.com', '6yXeZ9z6A8elhSPA2ogQJQkl7ydI6LnZqI7iWGCRsQMDtrEYdEcv82pmAT800vvx', '', 'Employee', '2025-03-31 18:48:46', 0, 1),
(22, 'David', 'Brown', 'david.brown@example.com', 'Vjwnn9CE7+J5izZ1V8aupAr+WYWGahP8SbTxt5jXG7UP6KoaCfrn129VO9S3wnnx', '', 'Employee', '2025-03-31 18:48:53', 0, 1),
(23, 'Emily', 'Davis', 'emily.davis@example.com', 'qOiJ5IoJSA8YqccH4D69RCqU4JIrbSoohAYpKfSqameCQRV9ClOhdZIrl0SPSbi3', '', 'Employee', '2025-03-31 18:49:00', 0, 1),
(24, 'Frank', 'Miller', 'frank.miller@example.com', 'mweP7z9G+jd8w1VJYBDvzP5ArwKVXMo+PGEXCErIGKSTUUgFW+sOfUrYrM/ImQOa', '', 'Employee', '2025-03-31 18:49:06', 0, 1),
(25, 'Grace', 'Wilson', 'grace.wilson@example.com', 'TUs28Bl9D3k5lxZDDTHhKG9xh3XS/lHWzp9posJ9x2+j9yludaz4Ar1DAcsGWwMG', '', 'Employee', '2025-03-31 18:49:12', 0, 1),
(26, 'Henry', 'Moore', 'henry.moore@example.com', 'CCMvMoQSa/+O7AB5vTwj5N0quuGZjxpjwBwAUaVjcrH/0gBx61aeZuVQYHY9i96p', '', 'Employee', '2025-03-31 18:49:17', 0, 1),
(27, 'Isabella', 'Taylor', 'isabella.taylor@example.com', 'W1nb7xYRtxZEmfmulAVseHBtYOXbyAwVADKxDCa/FOI2grBD2sXmgpLD7jgkQB+a', '', 'Employee', '2025-03-31 18:49:23', 0, 1),
(28, 'Jack', 'Anderson', 'jack.anderson@example.com', 'm29wYUZBdqxgUdmI8tjOHYsEwBq99jbAPLU77eef4LqZjcFmh1I1yJSYnA+9KRth', '', 'Employee', '2025-03-31 18:49:30', 0, 1),
(29, 'Acme', 'Corporation', 'hr@acmecorp.com', 'hBk1womJAguE2EA+hwxvdBoEHXKdJfGV6iIX2mf4X8Reo6cu6yw3cibDmzfgxfBX', '', 'Employer', '2025-03-31 19:25:54', 0, 1),
(30, 'Global', 'Industries', 'jobs@globalind.net', 'e07PA31kPwmyKJbUHbnzYvVJ5u8ozj8S5fZkeNt+Nu9rhtYAi9PnOvaraHlzDDg5', '', 'Employer', '2025-03-31 19:26:12', 0, 1),
(31, 'Innovate', 'Solutions', 'careers@innovatesol.org', 'V8Pqzesmb2Loqw0Lviwlx0z5PqyT0IibnVt1B/gwZqiMEthgM1rmO5bbJBuGZOaP', '', 'Employer', '2025-03-31 19:26:17', 0, 1),
(32, 'Tech', 'Forward', 'talent@techfwd.io', 'eRiIM62aIs8K2K5blfH6Yxvwc4KXt2c9vKFqkeEd3z+ccPyPvj1YBIYE5z4tBzb7', '', 'Employer', '2025-03-31 19:26:22', 0, 1),
(33, 'Bright', 'Horizons', 'apply@brighthorizons.co.uk', 'QkwvVhYKk4lszq88istrLGyKtp9IaLZcL81DDaDE3eVK5ZeTggX5o6HyGSFlEGvM', '', 'Employer', '2025-03-31 19:26:27', 0, 1);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `connections`
--
ALTER TABLE `connections`
  ADD PRIMARY KEY (`ConnectionID`),
  ADD UNIQUE KEY `idx_unique_connection` (`RequesterID`,`ReceiverID`),
  ADD KEY `ReceiverID` (`ReceiverID`);

--
-- A tábla indexei `employerrequest`
--
ALTER TABLE `employerrequest`
  ADD PRIMARY KEY (`ApplicantID`),
  ADD UNIQUE KEY `UserID_2` (`UserID`),
  ADD KEY `UserID` (`UserID`);

--
-- A tábla indexei `employers`
--
ALTER TABLE `employers`
  ADD PRIMARY KEY (`EmployerID`) USING BTREE,
  ADD UNIQUE KEY `UserID_2` (`UserID`),
  ADD KEY `UserID` (`UserID`);

--
-- A tábla indexei `messages`
--
ALTER TABLE `messages`
  ADD PRIMARY KEY (`MessageID`),
  ADD KEY `SenderID` (`SenderID`),
  ADD KEY `ReceiverID` (`ReceiverID`);

--
-- A tábla indexei `posts`
--
ALTER TABLE `posts`
  ADD PRIMARY KEY (`PostID`) USING BTREE,
  ADD KEY `EmployerID` (`EmployerID`);

--
-- A tábla indexei `profiles`
--
ALTER TABLE `profiles`
  ADD PRIMARY KEY (`ProfileID`),
  ADD KEY `UserID` (`UserID`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD UNIQUE KEY `Email_2` (`Email`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `connections`
--
ALTER TABLE `connections`
  MODIFY `ConnectionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `employerrequest`
--
ALTER TABLE `employerrequest`
  MODIFY `ApplicantID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `employers`
--
ALTER TABLE `employers`
  MODIFY `EmployerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT a táblához `messages`
--
ALTER TABLE `messages`
  MODIFY `MessageID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `posts`
--
ALTER TABLE `posts`
  MODIFY `PostID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT a táblához `profiles`
--
ALTER TABLE `profiles`
  MODIFY `ProfileID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `connections`
--
ALTER TABLE `connections`
  ADD CONSTRAINT `connections_ibfk_1` FOREIGN KEY (`RequesterID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  ADD CONSTRAINT `connections_ibfk_2` FOREIGN KEY (`ReceiverID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Megkötések a táblához `employerrequest`
--
ALTER TABLE `employerrequest`
  ADD CONSTRAINT `employerrequest_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Megkötések a táblához `employers`
--
ALTER TABLE `employers`
  ADD CONSTRAINT `employers_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Megkötések a táblához `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`SenderID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`ReceiverID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Megkötések a táblához `posts`
--
ALTER TABLE `posts`
  ADD CONSTRAINT `FK_Posts_Employers` FOREIGN KEY (`EmployerID`) REFERENCES `employers` (`EmployerID`);

--
-- Megkötések a táblához `profiles`
--
ALTER TABLE `profiles`
  ADD CONSTRAINT `profiles_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
