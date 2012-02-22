//  If using JQuery Javascript Library.

var NUMBER_OF_STARS = 5;
//  var ratingDescriptions = ["Aweful", "Silly", "Ok", "Good", "Awesome"];
var ratingDescriptions = ["Awefully Written", "Silly", "Ok", "Good read.", "Awesome. Very well written."];

//  var ratingPath = "./getputs/Images/Ratings/";
var ratingPath = "http://www.getputs.com/getputs/Images/Ratings/";
//  var ratingPath = "getputs\\Images\\Ratings\\";

function init_rating() 
{
    //  var ratings = document.getElementsByTagName('div');
    var ratings = document.getElementsByTagName('span');

    for (var i = 0; i < ratings.length; i++) 
    {    
        if (ratings[i].className != 'rating')
            continue;

        var rating = ratings[i].firstChild.nodeValue;
        ratings[i].removeChild(ratings[i].firstChild);
        
        if (rating > NUMBER_OF_STARS || rating < 0)
            continue;

        for (var j = 0; j < NUMBER_OF_STARS; j++) 
        {           
            var star = document.createElement('img');
            if (rating >= 1) 
            {
                //  star.setAttribute('src', './Images/Ratings/rating_on.gif');
                star.setAttribute('src', ratingPath + 'rating_on.gif');
                star.className = 'on';
                rating--;
            }
            else if (rating == 0.5) 
            //  else if(rating >= 0.25 && rating <= 0.75)
            {
                //  star.setAttribute('src', './Images/Ratings/rating_half.gif');
                star.setAttribute('src', ratingPath + 'rating_half.gif');
                star.className = 'half';
                rating = 0;
            }
            else 
            {
                //  star.setAttribute('src', './Images/Ratings/rating_off.gif');
                star.setAttribute('src', ratingPath + 'rating_off.gif');
                star.className = 'off';
            }
            
//            alert("rating: " + rating);
//            alert("src: " + star.getAttribute('src'));
//            alert("id: " + ratings[i].getAttribute('id'));
            
            var widgetId = ratings[i].getAttribute('id').substr(7);
            star.setAttribute('id', 'star_' + widgetId + '_' + j);

            var ratingLabel = document.getElementById("ratingLabel_" + widgetId);
            var ratingLabelText = ratingLabel.innerHTML;
            
            //  star.onmouseover = new Function("evt", "displayHover (" + widgetId + ", " + j + ", " + ratingLabelText + ");");
            //  star.onmouseout  = new Function("evt", "displayNormal(" + widgetId + ", " + j + ", " + ratingLabelText + ");");
           
            star.setAttribute('onmouseover', 'displayHover(' + widgetId + ', ' + j + ', ' + ratingLabelText + ');');
            star.setAttribute('onmouseout', 'displayNormal(' + widgetId + ', ' + j + ', ' + ratingLabelText + ');');
            
            ratings[i].appendChild(star);
        }
    }
}


function displayHover(ratingId, star, ratingLabelText) 
{    
    for (var i = 0; i <= star; i++) 
    {
        //  document.getElementById('star_' + ratingId + '_' + i).setAttribute('src', './Images/Ratings/rating_over.gif');
        
        document.getElementById('star_' + ratingId + '_' + i).setAttribute('src', ratingPath + 'rating_over.gif');
        var ratingLabel = document.getElementById("ratingLabel_" + ratingId);
        ratingLabel.innerHTML = "<font color=gray size=1.8>" + ratingDescriptions[star] + "</font>";
    }
}

function displayNormal(ratingId, star, ratingLabelText) 
{
    for (var i = 0; i <= star; i++) 
    {
        var status = document.getElementById('star_' + ratingId + '_' + i).className;        
        //  document.getElementById('star_' + ratingId + '_' + i).setAttribute('src', './Images/Ratings/rating_' + status + '.gif');
        
        document.getElementById('star_' + ratingId + '_' + i).setAttribute('src', ratingPath + 'rating_' + status + '.gif');
        var ratingLabel = document.getElementById("ratingLabel_" + ratingId);
        ratingLabel.innerHTML = "<font color=gray size=1.8>" + ratingLabelText + "</font>";
    }
}




/*
//  If using Prototype Javascript Library.

var NUMBER_OF_STARS = 5;
var ratingDescriptions = ["Aweful", "Silly", "Ok", "Good", "Awesome"];
var ratingAlready = "<font color=gray size=1.8>Thank You</font>";

function init_rating()
{
var ratings = document.getElementsByTagName('span');
for (var i = 0; i < ratings.length; i++)
{
if (ratings[i].className != 'rating')
continue;            
var rating = ratings[i].firstChild.nodeValue;
ratings[i].removeChild(ratings[i].firstChild);
if (rating > NUMBER_OF_STARS || rating < 0)
continue;
for (var j = 0; j < NUMBER_OF_STARS; j++)
{
var star = document.createElement('img');
if (rating >= 1)
{
star.setAttribute('src', './Images/Ratings/rating_on.gif');
star.className = 'on';
rating--;
}
else if(rating == 0.5)
{
star.setAttribute('src', './Images/Ratings/rating_half.gif');
star.className = 'half';
rating = 0;
}
else
{
star.setAttribute('src', './Images/Ratings/rating_off.gif');
star.className = 'off';
}
                        
var widgetId = ratings[i].getAttribute('id').substr(7);
star.setAttribute('id', 'star_'+widgetId+'_'+j);
            
var ratingLabel = document.getElementById("ratingLabel_" + widgetId);
var ratingLabelText = ratingLabel.innerHTML;
            
star.onmouseover = new Function("evt", "displayHover(" + widgetId + ", " + j + ", " + ratingLabelText + ");");
star.onmouseout  = new Function("evt", "displayNormal(" + widgetId + ", " + j + ", " + ratingLabelText + ");");
            
ratings[i].appendChild(star);
} 
}
}

function displayHover(ratingId, star, ratingLabelText)
{        
for (var i = 0; i <= star; i++)
{
document.getElementById('star_'+ratingId+'_'+i).setAttribute('src', './Images/Ratings/rating_over.gif');
star.className = 'over';
        
var ratingLabel = document.getElementById("ratingLabel_" + ratingId);
ratingLabel.innerHTML = "<font color=gray size=1.8>" + ratingDescriptions[star] + "</font>";
}
}

function displayNormal(ratingId, star, ratingLabelText)
{    
for (var i = 0; i <= star; i++)
{
var status = document.getElementById('star_'+ratingId+'_'+i).className;
document.getElementById('star_'+ratingId+'_'+i).setAttribute('src', './Images/Ratings/rating_'+status+'.gif');
        
var ratingLabel = document.getElementById("ratingLabel_" + ratingId);
ratingLabel.innerHTML = "<font color=gray size=1.8>" + ratingLabelText + "</font>";
}
}

*/
