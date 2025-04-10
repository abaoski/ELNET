
CREATE DATABASE VERDENEST;

USE VERDENEST

-- Create the USERS table
CREATE TABLE USERS (
    USER_ID INT IDENTITY(1,1) PRIMARY KEY,
    USERNAME VARCHAR(255) NOT NULL,
    PASSWORD_HASH VARCHAR(255) NOT NULL,
    EMAIL VARCHAR(255) NOT NULL,
    ROLE VARCHAR(100) NOT NULL,
    FIRST_NAME VARCHAR(100) NOT NULL,
    LAST_NAME VARCHAR(100) NOT NULL,
    PHONE_NUMBER VARCHAR(20),
    DATE_JOINED DATETIME DEFAULT GETDATE()
);
GO

-- Create the ANNOUNCEMENTS table
CREATE TABLE ANNOUNCEMENTS (
    ANNOUNCEMENT_ID INT IDENTITY(1,1) PRIMARY KEY,
    TITLE VARCHAR(255) NOT NULL,
    CONTENT TEXT NOT NULL,
    DATE_POSTED DATETIME DEFAULT GETDATE()
);
GO

-- Create the BILLING_AND_PAYMENTS table
CREATE TABLE BILLING_AND_PAYMENTS (
    BILL_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    BILL_TYPE VARCHAR(100) NOT NULL,
    AMOUNT_DUE DECIMAL(10, 2) NOT NULL,
    DUE_DATE DATE NOT NULL,
    PAYMENT_STATUS VARCHAR(20) NOT NULL,
    PAYMENT_DATE DATE,
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_PAYMENT_STATUS CHECK (PAYMENT_STATUS IN ('PENDING', 'PAID', 'OVERDUE'))
);
GO

-- Create the FACILITY_RESERVATIONS table
CREATE TABLE FACILITY_RESERVATIONS (
    RESERVATION_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    FACILITY_NAME VARCHAR(255) NOT NULL,
    RESERVATION_DATE DATE NOT NULL,
    START_TIME TIME NOT NULL,
    END_TIME TIME NOT NULL,
    STATUS VARCHAR(20) NOT NULL,
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_RESERVATION_STATUS CHECK (STATUS IN ('CONFIRMED', 'PENDING', 'CANCELLED'))
);
GO

-- Create the SERVICE_REQUESTS table
CREATE TABLE SERVICE_REQUESTS (
    REQUEST_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    REQUEST_TYPE VARCHAR(255) NOT NULL,
    DESCRIPTION TEXT NOT NULL,
    STATUS VARCHAR(20) NOT NULL,
    DATE_SUBMITTED DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_SERVICE_REQUEST_STATUS CHECK (STATUS IN ('PENDING', 'COMPLETED', 'IN-PROGRESS'))
);
GO

-- Create the DOCUMENTS table
CREATE TABLE DOCUMENTS (
    DOCUMENT_ID INT IDENTITY(1,1) PRIMARY KEY,
    TITLE VARCHAR(255) NOT NULL,
    DOCUMENT_TYPE VARCHAR(100) NOT NULL,
    FILE_PATH VARCHAR(255) NOT NULL,
    UPLOAD_DATE DATETIME DEFAULT GETDATE()
);
GO

-- Create the COMMUNITY_FORUM_POSTS table
CREATE TABLE COMMUNITY_FORUM_POSTS (
    POST_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    CONTENT TEXT NOT NULL,
    DATE_POSTED DATETIME DEFAULT GETDATE(),
    STATUS VARCHAR(20) NOT NULL,
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_FORUM_POST_STATUS CHECK (STATUS IN ('ACTIVE', 'ARCHIVED'))
);
GO

-- Create the VISITOR_PASS_REQUESTS table
CREATE TABLE VISITOR_PASS_REQUESTS (
    PASS_REQUEST_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    VISITOR_NAME VARCHAR(255) NOT NULL,
    VISIT_DATE DATE NOT NULL,
    APPROVAL_STATUS VARCHAR(20) NOT NULL,
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_APPROVAL_STATUS CHECK (APPROVAL_STATUS IN ('APPROVED', 'PENDING', 'REJECTED'))
);
GO

-- Create the EVENT_CALENDAR table
CREATE TABLE EVENT_CALENDAR (
    EVENT_ID INT IDENTITY(1,1) PRIMARY KEY,
    EVENT_NAME VARCHAR(255) NOT NULL,
    EVENT_DATE DATE NOT NULL,
    EVENT_TIME TIME NOT NULL,
    DESCRIPTION TEXT NOT NULL,
    LOCATION VARCHAR(255) NOT NULL
);
GO

-- Create the FEEDBACK_AND_COMPLAINTS table
CREATE TABLE FEEDBACK_AND_COMPLAINTS (
    FEEDBACK_ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT,
    TYPE VARCHAR(20) NOT NULL,
    MESSAGE TEXT NOT NULL,
    STATUS VARCHAR(20) NOT NULL,
    DATE_SUBMITTED DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (USER_ID) REFERENCES USERS(USER_ID),
    CONSTRAINT CHK_FEEDBACK_STATUS CHECK (STATUS IN ('OPEN', 'CLOSED', 'IN-PROGRESS')),
    CONSTRAINT CHK_FEEDBACK_TYPE CHECK (TYPE IN ('FEEDBACK', 'COMPLAINT'))
);
GO

-- Create the CONTACTS table
CREATE TABLE CONTACTS (
    CONTACT_ID INT IDENTITY(1,1) PRIMARY KEY,
    NAME VARCHAR(255) NOT NULL,
    ROLE VARCHAR(100) NOT NULL,
    PHONE_NUMBER VARCHAR(20),
    EMAIL VARCHAR(255),
    OFFICE_HOURS VARCHAR(100)
);
GO

-- Create the POLLS_AND_SURVEYS table
CREATE TABLE POLLS_AND_SURVEYS (
    POLL_ID INT IDENTITY(1,1) PRIMARY KEY,
    QUESTION TEXT NOT NULL,
    OPTIONS TEXT NOT NULL,
    DATE_CREATED DATETIME DEFAULT GETDATE()
);
GO
