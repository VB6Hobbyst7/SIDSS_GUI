Imports System.Data.SQLite
Imports System.Data
Public Class Create_Empty_SQL_Data_Tables
    Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
    Dim cmd As New SQLiteCommand

    Public Sub Create_empyt_tables()
        myConnection.Open()
        cmd.Connection = myConnection
        Dim cmd_string As String
        cmd_string = "
CREATE TABLE Ref_ET_Table (
    SNo          INTEGER      PRIMARY KEY
                              NOT NULL ON CONFLICT REPLACE
                              DEFAULT (1),
    Date         VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    StdTime      VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Tmid         VARCHAR (20),
    DOY          VARCHAR (5)  DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    AirTemp      VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    RH           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Rs           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    wind__spd    VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Sc           VARCHAR (20) NOT NULL ON CONFLICT REPLACE
                              DEFAULT (0),
    omega        VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    dr           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    delta__vapor VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    delta__angle VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    phi          VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    beta         VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    omega__s     VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    omega__1     VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    omega__2     VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Ra           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Rso          VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    fcd          VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    TKhr         VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    es           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    ea           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Rnl          VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Rns          VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    G            VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    P            VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    gamma        VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    u2           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Cn           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Cd           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    Rn           VARCHAR (20) DEFAULT (0) 
                              NOT NULL ON CONFLICT REPLACE,
    fcd_adv      VARCHAR (20) NOT NULL ON CONFLICT REPLACE
                              DEFAULT (0),
    ETo          VARCHAR (20),
    ETr          VARCHAR (20),
    Rs_Rso_adv   VARCHAR (20),
    Kd           VARCHAR (20),
    Kb           VARCHAR (20),
    Rso_adv      VARCHAR (20),
    W            VARCHAR (20),
    sin_phi      VARCHAR (20) 
);

CREATE TABLE Site_Info_Summary (
    SNo          INTEGER       PRIMARY KEY AUTOINCREMENT
                               NOT NULL
                               DEFAULT (0),
    SiteName     VARCHAR (50),
    Latitude     DECIMAL (10),
    Longitude    DECIMAL (10),
    Center_Longi DECIMAL (10),
    Elevation    DECIMAL (10),
    Z__t         DECIMAL (10),
    Z__u         DECIMAL (10),
    Summary      VARCHAR (500) 
);

CREATE TABLE WaterBalance_Table (
    SNo       INTEGER      PRIMARY KEY,
    Date      VARCHAR (50),
    DOY       VARCHAR (50),
    Precip    VARCHAR (50),
    Irrig     VARCHAR (50),
    [Tmax, F] VARCHAR (50),
    [Tmin, F] VARCHAR (50),
    GDD       VARCHAR (50),
    Kc        VARCHAR (50),
    ETr       VARCHAR (50),
    ETc       VARCHAR (50),
    Drz       VARCHAR (50),
    Dmax      VARCHAR (50),
    Di        VARCHAR (50) 
);

"
        cmd.CommandText = cmd_string
        cmd.ExecuteNonQuery()
        myConnection.Close()
    End Sub
End Class

