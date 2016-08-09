
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/09/2016 13:29:37
-- Generated from EDMX file: c:\users\haydencheers\documents\visual studio 2015\Projects\FleetServer\FleetServer\FleetServerModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Workstations'
CREATE TABLE [dbo].[Workstations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ComputerName] nvarchar(max)  NOT NULL,
    [IPAddress] nvarchar(max)  NOT NULL,
    [MACAddress] nvarchar(max)  NOT NULL,
    [LastSeen] datetime  NOT NULL
);
GO

-- Creating table 'WorkStationWorkGroups'
CREATE TABLE [dbo].[WorkStationWorkGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TimeRemoved] datetime  NULL,
    [CanShare] bit  NOT NULL,
    [WorkstationId] int  NOT NULL,
    [WorkGroupId] int  NOT NULL,
    [RoomId] int  NOT NULL
);
GO

-- Creating table 'WorkGroups'
CREATE TABLE [dbo].[WorkGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Started] datetime  NOT NULL,
    [Expired] datetime  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identifier] nvarchar(max)  NOT NULL,
    [RoleId] int  NOT NULL
);
GO

-- Creating table 'Rooms'
CREATE TABLE [dbo].[Rooms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identifier] nvarchar(max)  NOT NULL,
    [Building_Id] int  NOT NULL
);
GO

-- Creating table 'Buildings'
CREATE TABLE [dbo].[Buildings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identifier] nvarchar(max)  NOT NULL,
    [CampusId] int  NOT NULL
);
GO

-- Creating table 'Campus'
CREATE TABLE [dbo].[Campus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identifier] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Messages'
CREATE TABLE [dbo].[Messages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TargetApplication] nvarchar(max)  NOT NULL,
    [TimeSent] datetime  NOT NULL
);
GO

-- Creating table 'WorkStationMessages'
CREATE TABLE [dbo].[WorkStationMessages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ReceiverId] nvarchar(max)  NOT NULL,
    [MessageId] nvarchar(max)  NOT NULL,
    [HasBeenSeen] bit  NOT NULL,
    [Received] nvarchar(max)  NOT NULL,
    [MessageId1] int  NOT NULL,
    [WorkstationId] int  NOT NULL
);
GO

-- Creating table 'Applications'
CREATE TABLE [dbo].[Applications] (
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Messages_FileMessage'
CREATE TABLE [dbo].[Messages_FileMessage] (
    [FileType] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FileSizeBytes] int  NOT NULL,
    [HasBeenScanned] bit  NOT NULL,
    [URI] nvarchar(max)  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Messages_AppMessage'
CREATE TABLE [dbo].[Messages_AppMessage] (
    [MessageContents] nvarchar(max)  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'UserWorkGroup'
CREATE TABLE [dbo].[UserWorkGroup] (
    [Users_Id] int  NOT NULL,
    [WorkGroups_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Workstations'
ALTER TABLE [dbo].[Workstations]
ADD CONSTRAINT [PK_Workstations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkStationWorkGroups'
ALTER TABLE [dbo].[WorkStationWorkGroups]
ADD CONSTRAINT [PK_WorkStationWorkGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkGroups'
ALTER TABLE [dbo].[WorkGroups]
ADD CONSTRAINT [PK_WorkGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rooms'
ALTER TABLE [dbo].[Rooms]
ADD CONSTRAINT [PK_Rooms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Buildings'
ALTER TABLE [dbo].[Buildings]
ADD CONSTRAINT [PK_Buildings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Campus'
ALTER TABLE [dbo].[Campus]
ADD CONSTRAINT [PK_Campus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages'
ALTER TABLE [dbo].[Messages]
ADD CONSTRAINT [PK_Messages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkStationMessages'
ALTER TABLE [dbo].[WorkStationMessages]
ADD CONSTRAINT [PK_WorkStationMessages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Applications'
ALTER TABLE [dbo].[Applications]
ADD CONSTRAINT [PK_Applications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages_FileMessage'
ALTER TABLE [dbo].[Messages_FileMessage]
ADD CONSTRAINT [PK_Messages_FileMessage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Messages_AppMessage'
ALTER TABLE [dbo].[Messages_AppMessage]
ADD CONSTRAINT [PK_Messages_AppMessage]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Users_Id], [WorkGroups_Id] in table 'UserWorkGroup'
ALTER TABLE [dbo].[UserWorkGroup]
ADD CONSTRAINT [PK_UserWorkGroup]
    PRIMARY KEY CLUSTERED ([Users_Id], [WorkGroups_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [WorkstationId] in table 'WorkStationWorkGroups'
ALTER TABLE [dbo].[WorkStationWorkGroups]
ADD CONSTRAINT [FK_WorkstationWorkStationWorkGroup]
    FOREIGN KEY ([WorkstationId])
    REFERENCES [dbo].[Workstations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkstationWorkStationWorkGroup'
CREATE INDEX [IX_FK_WorkstationWorkStationWorkGroup]
ON [dbo].[WorkStationWorkGroups]
    ([WorkstationId]);
GO

-- Creating foreign key on [WorkGroupId] in table 'WorkStationWorkGroups'
ALTER TABLE [dbo].[WorkStationWorkGroups]
ADD CONSTRAINT [FK_WorkGroupWorkStationWorkGroup]
    FOREIGN KEY ([WorkGroupId])
    REFERENCES [dbo].[WorkGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkGroupWorkStationWorkGroup'
CREATE INDEX [IX_FK_WorkGroupWorkStationWorkGroup]
ON [dbo].[WorkStationWorkGroups]
    ([WorkGroupId]);
GO

-- Creating foreign key on [Users_Id] in table 'UserWorkGroup'
ALTER TABLE [dbo].[UserWorkGroup]
ADD CONSTRAINT [FK_UserWorkGroup_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [WorkGroups_Id] in table 'UserWorkGroup'
ALTER TABLE [dbo].[UserWorkGroup]
ADD CONSTRAINT [FK_UserWorkGroup_WorkGroup]
    FOREIGN KEY ([WorkGroups_Id])
    REFERENCES [dbo].[WorkGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserWorkGroup_WorkGroup'
CREATE INDEX [IX_FK_UserWorkGroup_WorkGroup]
ON [dbo].[UserWorkGroup]
    ([WorkGroups_Id]);
GO

-- Creating foreign key on [RoleId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_RoleUser]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoleUser'
CREATE INDEX [IX_FK_RoleUser]
ON [dbo].[Users]
    ([RoleId]);
GO

-- Creating foreign key on [RoomId] in table 'WorkStationWorkGroups'
ALTER TABLE [dbo].[WorkStationWorkGroups]
ADD CONSTRAINT [FK_RoomWorkStationWorkGroup]
    FOREIGN KEY ([RoomId])
    REFERENCES [dbo].[Rooms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoomWorkStationWorkGroup'
CREATE INDEX [IX_FK_RoomWorkStationWorkGroup]
ON [dbo].[WorkStationWorkGroups]
    ([RoomId]);
GO

-- Creating foreign key on [Building_Id] in table 'Rooms'
ALTER TABLE [dbo].[Rooms]
ADD CONSTRAINT [FK_RoomBuilding]
    FOREIGN KEY ([Building_Id])
    REFERENCES [dbo].[Buildings]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RoomBuilding'
CREATE INDEX [IX_FK_RoomBuilding]
ON [dbo].[Rooms]
    ([Building_Id]);
GO

-- Creating foreign key on [CampusId] in table 'Buildings'
ALTER TABLE [dbo].[Buildings]
ADD CONSTRAINT [FK_CampusBuilding]
    FOREIGN KEY ([CampusId])
    REFERENCES [dbo].[Campus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CampusBuilding'
CREATE INDEX [IX_FK_CampusBuilding]
ON [dbo].[Buildings]
    ([CampusId]);
GO

-- Creating foreign key on [MessageId1] in table 'WorkStationMessages'
ALTER TABLE [dbo].[WorkStationMessages]
ADD CONSTRAINT [FK_MessageWorkStationMessage]
    FOREIGN KEY ([MessageId1])
    REFERENCES [dbo].[Messages]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageWorkStationMessage'
CREATE INDEX [IX_FK_MessageWorkStationMessage]
ON [dbo].[WorkStationMessages]
    ([MessageId1]);
GO

-- Creating foreign key on [WorkstationId] in table 'WorkStationMessages'
ALTER TABLE [dbo].[WorkStationMessages]
ADD CONSTRAINT [FK_WorkstationWorkStationMessage]
    FOREIGN KEY ([WorkstationId])
    REFERENCES [dbo].[Workstations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkstationWorkStationMessage'
CREATE INDEX [IX_FK_WorkstationWorkStationMessage]
ON [dbo].[WorkStationMessages]
    ([WorkstationId]);
GO

-- Creating foreign key on [Id] in table 'Messages_FileMessage'
ALTER TABLE [dbo].[Messages_FileMessage]
ADD CONSTRAINT [FK_FileMessage_inherits_Message]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Messages]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Messages_AppMessage'
ALTER TABLE [dbo].[Messages_AppMessage]
ADD CONSTRAINT [FK_AppMessage_inherits_Message]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Messages]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------