-- SQL Script
-- Created on 12/04/2025 by querzion

-- Write your SQL queries below:
insert into NotificationTargetGroups (Id, TargetGroup)
values ('1', 'AllUsers'),
       ('2', 'Admins'),
       ('3', 'Managers');

insert into NotificationTypes (Id, NotificationType)
values ('1', 'User'),
    ('2', 'Project'),
    ('3', 'Client');

insert into Tags (Id, TagName)
values ('1', 'Computers'),
       ('2', 'Finance'),
       ('3', 'Marketing'),
       ('4', 'HR'),
       ('5', 'Social'),
       ('6', 'Economics'),
       ('7', 'IT'),
       ('8', 'AI'),
       ('9', 'Data'),
       ('10', 'Programming');

insert into Statuses (Id, StatusName)
values ('1', 'Started'),
       ('2', 'Paused'),
       ('3', 'Not Started'),
       ('4', 'Completed');

-- DATABASE TIMESTAMP ERRORS
UPDATE Projects
SET Created = datetime(Created / 1000, 'unixepoch'),
    StartDate = datetime(StartDate / 1000, 'unixepoch'),
    EndDate = datetime(EndDate / 1000, 'unixepoch');

UPDATE ImagePaths
SET UploadedAt = datetime(UploadedAt / 1000, 'unixepoch')
WHERE UploadedAt > 1000000000000;