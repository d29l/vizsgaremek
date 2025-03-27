-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 27. 22:03
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
(1, 1, 'Voomm', '8061 Weeping Birch Street', '', 0, 'n/a', 'http://people.com.cn/massa.json?neque=in&libero=felis&convallis=eu', 'Phasellus sit amet erat. Nulla tempus. Vivamus in felis eu sapien cursus vestibulum.', 2006),
(2, 2, 'Ntags', '8709 Moulton Place', '', 0, 'Business Services', 'https://lycos.com/sed/sagittis/nam/congue.aspx?nulla=quam&suspendisse=nec&potenti=dui&cras=luctus&in=rutrum&purus=nulla&eu=tellus&magna=in&vulputate=sagittis&luctus=dui&cum=vel&sociis=nisl&natoque=duis&penatibus=ac&et=nibh&magnis=fusce&dis=lacus&parturien', 'Fusce consequat. Nulla nisl. Nunc nisl.', 2007),
(3, 3, 'Youfeed', '62 Esch Junction', '', 0, 'n/a', 'https://patch.com/a/suscipit/nulla/elit/ac.js?vel=porta&augue=volutpat&vestibulum=quam&ante=pede&ipsum=lobortis&primis=ligula&in=sit&faucibus=amet&orci=eleifend&luctus=pede&et=libero&ultrices=quis&posuere=orci&cubilia=nullam&curae=molestie&donec=nibh&phar', 'Duis consequat dui nec nisi volutpat eleifend. Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus.', 2012),
(4, 4, 'Bubblemix', '61305 7th Point', '', 0, 'Major Pharmaceuticals', 'https://ebay.com/elit/ac.aspx?justo=nisl&eu=duis&massa=bibendum&donec=felis&dapibus=sed&duis=interdum&at=venenatis&velit=turpis&eu=enim&est=blandit&congue=mi&elementum=in&in=porttitor&hac=pede&habitasse=justo&platea=eu&dictumst=massa&morbi=donec&vestibulu', 'Nulla ut erat id mauris vulputate elementum. Nullam varius. Nulla facilisi.', 1990),
(5, 5, 'Ooba', '4 Hintze Alley', '', 0, 'Major Chemicals', 'https://liveinternet.ru/dui/maecenas/tristique/est/et/tempus.aspx?enim=felis&blandit=ut&mi=at&in=dolor&porttitor=quis&pede=odio&justo=consequat&eu=varius&massa=integer&donec=ac&dapibus=leo&duis=pellentesque&at=ultrices&velit=mattis&eu=odio&est=donec&congu', 'In congue. Etiam justo. Etiam pretium iaculis justo.', 1978),
(6, 6, 'Torol', 'Miskolc Buza Ter', '', 0, 'n/a', 'https://youtube.com', 'Lorem Ipsum', 2002),
(11, 11, 'Temp', 'Temp', '', 0, 'Temp', 'Temp', 'Temp', 2000);

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
(1, 1, 1, 'Nurse Practicioner', '', '', 'In congue. Etiam justo. Etiam pretium iaculis justo.', '2025-01-14 10:00:00'),
(2, 2, 2, 'Senior Editor', '', '', 'Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi.', '2024-12-25 08:30:15'),
(3, 3, 3, 'Senior Sales Associate', '', '', 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin risus. Praesent lectus.', '2023-07-04 14:45:30'),
(4, 4, 4, 'Food Chemist', '', '', 'Sed sagittis. Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus.', '2022-11-01 16:00:00'),
(5, 5, 5, 'Media Manager I', '', '', 'Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus vestibulum sagittis sapien. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.', '2021-05-20 09:15:45'),
(9, 11, 11, 'string34', '', '', 'string', '2025-03-20 10:05:59'),
(10, 11, 14, 'string', '', '', 'string', '2025-03-21 15:15:41'),
(13, 11, 11, 'string', '', '', 'string', '2025-03-21 15:44:36'),
(14, 11, 11, 'Test', '', '', 'Test', '2025-03-21 15:44:44'),
(15, 11, 11, 'string', '', '', 'string', '2025-03-23 19:05:13');

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
(1, 10, 'John Admin', 'Lorem Ipsum', 'Miskolc', 'https://i.pinimg.com/736x/d0/cb/d1/d0cbd1380c72ddf3750c896433b2dea1.jpg');

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
  `IsActive` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`UserID`, `FirstName`, `LastName`, `Email`, `Password`, `RefreshToken`, `Role`, `CreatedAt`, `IsActive`) VALUES
(1, '', '', 'iayrs0@g.co', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employer', '2025-02-01 20:45:36', 1),
(2, '', '', 'cewbach1@printfriendly.com', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employer', '2024-03-30 20:45:49', 0),
(3, '', '', 'ssparkwill2@myspace.com', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employee', '2023-01-28 20:46:02', 0),
(4, '', '', 'ctapply3@topsy.com', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employer', '2024-12-17 20:46:16', 1),
(5, '', '', 'iferbrache4@china.com.cn', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employee', '2023-05-12 11:46:26', 1),
(6, '', '', 'torol@gmail.com', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employer', '2024-12-01 12:10:40', 1),
(8, 'Bodi', 'Gusztika', 'utalomazadofizetest@gmail.com', 'QCMhWNTszTuPsiK8FEZqi3EBU/o1gnwq6e0h89Z8VQu/Id6N6kLtbmPyXp1+WXrx', '', 'Employee', '2025-02-18 21:04:44', 1),
(10, 'admin', 'admin', 'admin', 'UMpb+AnYxJl3A22rnZE3nvomoYp1Khg3FwAjGAu31GaSxkoT5J5+k6fc0FEun/AX', 'UIbDvVJYLa9+iBHMHTOG9PBTVsS52v1a7XIbSD6xPyR6tXaYbSyMZVLMoEdelQuHTwi1H5+D04zl13vtqMwjzw==', 'Admin', '2025-02-19 23:00:08', 1),
(11, 'admin', 'admin', 'admin2', 'X0bSYAMcD5utwSGpSYTLXiTz5INqbVFoDUpmVicNktqAkOuOjSbGdjNcoQ0lo/wI', 'CtmCpxCYDNsdsHZOydLJsxQBL4k33hSj9SzC2VPWZ2ljlWmXaJDA5qF4yGVT+fmW6d9YXZF0tXfCyXeblNbcsA==', 'Employer', '2025-02-19 23:02:26', 1);

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
  MODIFY `EmployerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT a táblához `messages`
--
ALTER TABLE `messages`
  MODIFY `MessageID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `posts`
--
ALTER TABLE `posts`
  MODIFY `PostID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `profiles`
--
ALTER TABLE `profiles`
  MODIFY `ProfileID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

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
