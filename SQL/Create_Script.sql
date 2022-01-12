USE [master]
GO

/****** Object:  Database [VBS_MPI_Data]    Script Date: 1/11/2022 4:51:52 PM ******/
DROP DATABASE [VBS_MPI_Data]
GO

/****** Object:  Database [VBS_MPI_Data]    Script Date: 1/11/2022 4:51:52 PM ******/
CREATE DATABASE [VBS_MPI_Data]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'VBS_MPI_Data', FILENAME = N'C:\Users\OITORLWestbK\VBS_MPI_Data.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'VBS_MPI_Data_log', FILENAME = N'C:\Users\OITORLWestbK\VBS_MPI_Data_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [VBS_MPI_Data].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [VBS_MPI_Data] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET ARITHABORT OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [VBS_MPI_Data] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [VBS_MPI_Data] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [VBS_MPI_Data] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET  ENABLE_BROKER 
GO

ALTER DATABASE [VBS_MPI_Data] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [VBS_MPI_Data] SET READ_COMMITTED_SNAPSHOT ON 
GO

ALTER DATABASE [VBS_MPI_Data] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [VBS_MPI_Data] SET  MULTI_USER 
GO

ALTER DATABASE [VBS_MPI_Data] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [VBS_MPI_Data] SET DB_CHAINING OFF 
GO

ALTER DATABASE [VBS_MPI_Data] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [VBS_MPI_Data] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [VBS_MPI_Data] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [VBS_MPI_Data] SET QUERY_STORE = OFF
GO

USE [VBS_MPI_Data]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [VBS_MPI_Data] SET  READ_WRITE 
GO


