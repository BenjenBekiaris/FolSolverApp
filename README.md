# Interactive Proof System Using Resolution Method in First Order Logic
This tool processes formula in language of first order logic for resolution and allows user to complete the proof in interactive way

This tool was created as teaching tool in order to help understand process of resolution in first order logic.
User is able to input formula to process, check the steps required to prepare formula for resolution and then solve the proof
by choosing clauses for resolution.

> Application is available online at https://benjenbekiaris.github.io/FolSolverApp/

## Setup
If you want to run the application locally, I highly recommend using Visual Studio (https://visualstudio.microsoft.com),
as whole project is coded using C# in .NET and Blazor. 

After installing the IDE of choice (i will reffer to Visual Studio in next steps) clone content of the master branch to your 
local device.

## Running the application locally

After completing the two steps of setup, open the whole solution in Visual Studio and you are ready to go. Solution contains
two projects:
1. FolSolverCore - responsible for most logic processing
2. FolSolverApp - user web interface

To check your setup, build and run the FolSolverApp project (green arrow at the topbar). At this point you have access to 
all the code fueling the application.

## Publishing as static website

To publish the app navigate to topbar, select Build and Publish. Make sure that you are publishing App and not the Core (this
is easily spotted, as in the case of trying to publish Core project, the option will be stated as publish the selection instead
of full name of project). Openning any file from FolSolverApp project will change the option of publishing to App.

Choose the required way of publishing, as it depends on the way you intend to host the website. In case you want to use GitHub Pages
(as I did), you need to publish the project to folder of your choosing.

Publishing the folder will create necessary files of website. When hosting on GitHub Pages, be awayre of these extra steps 
necessary for success:
1. repository with website files must contain .gitattribute telling the host that all the files are already in binary. 
This can be done via PowerShell by typing "* binary" >> .gitattributes .
3. repository need to contain empty .nojekyll file.
4. you need to rewrite the base href atribute in index.html to "/FolSolverApp/".

After taking the necessary steps, you can set up the GitHub Pages and you are done.
