/*****************************************************************************************************************************************
* Facebook Reader
* Basic C# software to visualize the stream of Facebook Fan Pages on your Desktop
* 
******************************************************************************************************************************************
* This is open source software. Open source, means you can see the source code. It also means free software. 
* However, free software doesn't mean the same as free beer. Free software is closer to free speech, free thinker, and freedom in general.
* Free software doesn't mean that you don't have to acknowledge the author and abide to their license.               
* The author is releasing this for learning/educational purposes and/or in the hope it will be useful and with NO WARRANTY OF ANY KIND.
* 
*******************************************************************************************************************************************
* This program is free software: you can redistribute it and/or modify 
* it under the terms of the GNU General Public License as published by 
* the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
* **
* This program is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of 
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
* GNU General Public License for more details.
* **
* You should have received a copy of the GNU General Public License 
* along with this program. If not, see <http://www.gnu.org/licenses/>. 
* *
 * ********************************************************************************************************************************** *
 *** ^                                                     Copyright (c) Nicla Rossini 2013                                       ^  ***
 * ********************************************************************************************************************************** *
 *          As long as you KEEP ALL NOTICES INTACT and DO NOT USE IT for COMMERCIAL software, you can do with this what you want.
 *              Please remember that PORTING counts as modification is thus subject to this same copyright and license.
 *              
 **************************************************************************************************************************************/

/* This is for Desktop Applications. If you want, it is better to use it with Windows Forms or anything else. 
 * If you plan to play with C# and VisualStudio, this goes in the Program.cs file.
 * 
 * Let's get started...*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Web;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
//using IWshRuntimeLibrary; I forget why I put this here

namespace Facebook
{
    class Program
    {

        static void Main(string[] args)
        {
       //let's set some variables
            string token1 = " "; // put your access token here
            string token2 = " "; //put yor access token secret here

        //now we build an url. This we need to get the authorization token
            string url = "https://graph.facebook.com/oauth/access_token?type=client_cred&client_id=" + token1 + "&client_secret=" + token2;
        
       //let's call Facebook, shall we? This is an http request
            var Requestit = (HttpWebRequest)WebRequest.Create(url);
            WebResponse datao = Requestit.GetResponse();
            System.IO.Stream status1 = datao.GetResponseStream();

       //will Facebook answer? By now we know. If everything is all right, we read the stream

            StreamReader sy = new StreamReader(status1);
            string text = sy.ReadToEnd(); //keep this variable in mind we'll use it much later
            StringBuilder output = new StringBuilder();
       
       //this is the simplest input, at least for me. You may need a different thing. Maybe a windows form 
            string patps = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //the machine's desktop
            string patpsas = patps + @"\FBaccounts.txt"; //or whatever else

      
            if (!System.IO.File.Exists(patpsas))
            {  //we're looking for a file in the Desktop. If it doesn't exist...
                Console.WriteLine("\a\n \t **I CAN'T FIND THE PAGES YOU WANT TO SEE!!**\n\n\n Don't panic just do this:\n\n 1) Put all fan pages into a text (.txt) file separated by a tab \n(no new line please!).\n\n 2) Then save the file on your Desktop with the name FBaccounts.txt\n\n d) Just run me again.\n\n\n\n P.S.: Finger crossing, knocks on wood, or even prayers are perfectly fine but\n won't help much \n");
                Console.ReadKey(); //this I love: it just keep the screen on hold until you press a random key
            }
            else
            {
                //leeeet's debug sigh. This thing should be working but I must have misplaced a bracket somewhere...
                //else (1)
                Console.WriteLine("\tWorking on it...");

              /* if instead the file exist we open it, reall all lines ...*/
                string[] gpages = System.IO.File.ReadAllLines(patpsas);

             /* ...and for each line */ 
                foreach (string page in gpages)
                {//foreach (2)

               /* we write a simple regex that isolates the relevant text. The text is separated by a tab */
                    string[] pages = page.Split('\t');

              /* now we need to further split the string to isolate the text that comes after each slash*/
                    foreach (string pag in pages)
                    { //foreach (3)
                        string[] pieces = pag.Split('/');
                        var ScreenN = pieces[3]; //and we tell the system that the information we need is after the third slash. You need to add some debug here and check if the thing you got is integral
                        string Newur = "https://graph.facebook.com/" + ScreenN; //now we build a new url

                        System.Threading.Thread.Sleep(6000); //put it to sleep a bit, otherwise we upset Facebook...

                       /* By now you'll have figured our that this is an http request so we're calling Facebook again */
                        var requestinfo = WebRequest.Create(Newur);
                        requestinfo.ContentType = "application/json; charset=utf-8"; 
                        requestinfo.Method = WebRequestMethods.Http.Get;
                        var responseinfo = (HttpWebResponse)requestinfo.GetResponse();
                        System.IO.Stream respoid = responseinfo.GetResponseStream();
                        StreamReader sysio = new StreamReader(respoid);
                        var jsonDataid = sysio.ReadToEnd();


                        JavaScriptSerializer sera = new JavaScriptSerializer(); //we need to deserialize the JSON now
                        dynamic item = sera.Deserialize<object>(jsonDataid);
                        string pageid = item["id"]; //and all we need is the id of the Fan Page
                       
                    /* now things get really interesting: we start to build the call to get the Fan Pages' feed.
                       First, we need to check what time is it and what day is today. Then, we subtract one day 
                       (to get yesterday at this same hour)  */

                        DateTime currentDate = DateTime.Now.AddDays(-1);

                      // then we need to convert the date and time to a unix timestamp (thanks to a friend for correcting this)
                        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);
                        TimeSpan duration = currentDate - epoch;
                        int seconds = (int)duration.TotalSeconds;

                       //now we finally bild the URL. Did you take note of the variable "text", btw?

                        string getstuff = "https://graph.facebook.com/" + pageid + "/feed?" + text + "&since=" + seconds;
                        // Console.WriteLine(getstuff);  for debug
                       
                      // and again, we call Facebook
                        var request = WebRequest.Create(getstuff);
                        request.ContentType = "application/json; charset=utf-8";
                        request.Method = WebRequestMethods.Http.Get;
                        var response = (HttpWebResponse)request.GetResponse();
                        System.IO.Stream respo = response.GetResponseStream();
                        StreamReader sys = new StreamReader(respo);
                        var jsonData = sys.ReadToEnd();

                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        dataList data = new JavaScriptSerializer().Deserialize<dataList>(jsonData); 

                        /* and now we deserialize the JSON */
                        foreach (var obj in data.data)
                        { //foreach (4)
                          /*
                           * I have decided to set some viariables
                           */
                            var damnedid = obj.id; //this is the post id. It's all you need to build the url of the post and to make further queries.
                            var dates = DateTime.Now; //don't need to tell yow what's this
                            var posterid = obj.from.id; //this is the id of the person who posts
                            var postedName = obj.from.name; //this is the name of the person who posts
                            var statusTitle = obj.name; // I forget what this is
                            var statusMessage = obj.message; //status or comment on link
                            var statusLink = obj.link;//enclosed link if you posted a link
                            var acti = obj.actions; //pointless, because it is better to make yet another call
                            var objtype = obj.type; //this tell you what kind of object that is
                            var statustype = obj.status_type; //what kind of status
                            var embeddedImage = obj.picture; //picture. You can embed this if you export the feed - just like I do for minagrey on mina-grey.com
                            var createdTime = obj.created_time; //any post has datetime stamp. This tells you the date and time
                            var updatedTime = obj.updated_time; //any post can now be modified. This tells you when it was modified
                            var applicationi = obj.application.id; // we can post with applications. If so, you'll see the id of the app here
                            var applicationn = obj.application.name; //  we can post with applications. This tells you the name of the app here

                            Console.WriteLine ("Date:"+dates +"/n"+"Post id:" + damnedid + "/n" + "Poster:"+ postedName +"/n" +"Message:" + StatusMessage +"/n" + "Link:" + statusLink  //etc. etc. etc.);
                                /* you can also add it to a database if you need it for some reason. Or write it to a document. If you want, you might print the status with a button to like it or share it.
                                   Now we complicate things and try to get the number of likes and comments. 
                                 */
                            Console.WriteLine("IF YOU SEE A LOOP OF JSON OBJECTS FOR LIKES, IT MEANS IT WORKS (NO MATTER IF IT'EMPTY):\n\n");
                            System.Threading.Thread.Sleep(3000); //put it to sleep a bit, otherwise we upset Facebook...
                            //We call Facebook again

                            string getdamnedlikes = "https://graph.facebook.com/" + damnedid + "/likes?" + text; //this is the url for likes
                            System.Threading.Thread.Sleep(3000);
                            var damnedrequest = WebRequest.Create(getdamnedlikes);
                            damnedrequest.ContentType = "application/json; charset=utf-8";
                            damnedrequest.Method = WebRequestMethods.Http.Get;
                            var damnedresponse = (HttpWebResponse)damnedrequest.GetResponse();
                            System.IO.Stream damnedrespo = damnedresponse.GetResponseStream();
                            StreamReader damnit = new StreamReader(damnedrespo);
                            var commentsData = damnit.ReadToEnd();
                            Console.WriteLine ("JSON OBJECT FOR LIKES ON THIS POST:\n"+commentsData); //for debug. It works.
                            
                            // now deserialize
                           JavaScriptSerializer whythef = new JavaScriptSerializer();
                           likeList doit = new JavaScriptSerializer().Deserialize<likeList>(commentsData); //this doesn't seem to work.

                            
                          if (doit.likes != null) //I've added some debug to check if there's something...
                          {
                                foreach (var likesonstuff in doit.likes)
                                {//foreach (7)
                                    //you can check if it's empty by doing this 

                                    // if (likesonstuff.id != null)
                                    //{
                                    var likeid = likesonstuff.ldata.id;
                                    Console.WriteLine(likeid);

                                    //}
                                    // if (likesonstuff.name != null)
                                    //{
                                    var likerid = likesonstuff.ldata.name;
                                    Console.WriteLine(likerid);

                                    
                                    
                                
                                        } //end foreach

                            } //end if

                            /* Now let's try with comments */
                          Console.WriteLine ("IF YOU SEE A LOOP OF JSON OBJECTS FOR COMMENTS, IT MEANS IT WORKS (NO MATTER IF IT'EMPTY):\n\n");
                          string getdamnedcomments = "https://graph.facebook.com/" + damnedid + "/comments?" + text; //this is the url for likes
                          System.Threading.Thread.Sleep(3000);
                          var damnedMS = WebRequest.Create(getdamnedcomments);
                          damnedMS.ContentType = "application/json; charset=utf-8";
                          damnedMS.Method = WebRequestMethods.Http.Get;
                          var damneddata = (HttpWebResponse)damnedMS.GetResponse();
                          System.IO.Stream ddata = damneddata.GetResponseStream();
                          StreamReader damnit02 = new StreamReader(ddata);
                          var commentsData02 = damnit02.ReadToEnd();
                          Console.WriteLine("COMMENTS JSON \n" + commentsData02 +"\n\tPRESS ANY KEY TO CONTINUE");
                          Console.ReadKey(); //this just reads the keyboard input. Press any key to continue

                            if (commentsData02 !=null){
                          JavaScriptSerializer whythismess = new JavaScriptSerializer();
                          commentList doitagain = new JavaScriptSerializer().Deserialize<commentList>(commentsData02); //this doesn't work. I have tried everything.
                            /*this breaks if there are no comments at all so I've added some sort of debug */
                          if (doitagain.comments != null) 
                          {
                              foreach (var commentsonstuff in doitagain.comments)
                              {//foreach (7)
                                  //you can check if it's empty by doing this 

                                  // if (likesonstuff.id != null)
                                  //{
                                  var commntid = commentsonstuff.cdata.id; //comment id
                                  var commentmessage = commentsonstuff.cdata.message;
                                  var likes4comment = commentsonstuff.cdata.like_count; //integral, but see below
                                  var user_likes = commentsonstuff.cdata.user_likes; //boolean, but why make life sour
                                  Console.WriteLine("Debugging the JSON for comments " + commntid+commentmessage+user_likes+likes4comment);

                                  //}
                                  // if (likesonstuff.name != null)
                                  //{
                                




                              } //end foreach

                          } //end if

                        }

                    }

                }
                
            }

            }
                
         

            // End of debug.
       /*P.S.: if you want more, you can always call the API with this url "http://graph.facebook.com/"+ postid +"?" + text */
            Console.ReadKey(); // this just reads the keyboard input. If you press a key, they close it.


        }

    }

}




public class data
{
    public string name { get; set; }
    public string id { get; set; }
    public from from { get; set; }
    public string message { get; set; }
    public actions actions { get; set; }
    public string type { get; set; }
    public string link { get; set; }
    public string picture { get; set; }
    public string caption { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
    public string status_type { get; set; }
    public application application { get; set; }
    public string created_time { get; set; }
    public string updated_time { get; set; }
   
}

public class from
{
    public string id { get; set; }
    public string name { get; set; }
}

public class actions
{
    public string name { get; set; }
    public string link { get; set; }
}

public class application
{
    public string name { get; set; }
    public string id { get; set; }
}

public class dataList
{
    public string dataType { get; set; }
    public List<data> data { get; set; }

}

public class likeList
{
    public string likeType { get; set; }
    public List<likes> likes { get; set; }

}



public class items
{
    public string id { get; set; }
}

//the classes for comments and likes may be wrong... I tried everything I know, and this is for educational purposes only, and so... :)

    public class comments
    {
        public cdata cdata { get; set; }
    }

 public class cdata{
        public string id { get; set; }
        public commentfrom commentfrom { get; set; }
        public string message { get; set; }
        public string canremove { get; set; }
        public string created_time { get; set; }
        public string like_count { get; set; }
        public string user_likes { get; set; }
    }


    public class commentfrom {
        public string category { get; set; }
        public string name { get; set;}
        public string id { get; set;}
        
    }

    public class likes
    {
        public ldata ldata { get; set; }
    }

public class ldata {
    public string id { get; set; }
    public string name { get; set; }
    }

  
//while the classes above do not seem to work,the ones below do work as expected.

public class iddataList
{

    public List<items> items { get; set; }
}

public class commentList
{
    public string commentType { get; set; }
    public List<comments> comments { get; set; }

}


    
