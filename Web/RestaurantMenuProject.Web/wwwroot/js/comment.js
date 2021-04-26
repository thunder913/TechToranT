
// Getting the id of the element
const segments = new URL(window.location.href).pathname.split('/');
const id = segments.pop() || segments.pop(); // Handle potential trailing slash

var starRatingControl = new StarRating('.star-rating', {
    maxStars: 5
});

$('.addComment').click(function (e) {
    e.preventDefault();
    var form = e.target.parentElement;
    var formdata = {
        'foodId': form.querySelector('input[name=id]').value,
        'foodType': Number(form.querySelector('input[name=foodType]').value),
        'rating': form.querySelector('.star-rating').value,
        'comment': form.querySelector('textarea[name=comment]').value
    }

    var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();

    $.ajax({
        type: 'POST',
        url: '/api/Comment/AddComment',
        data: JSON.stringify(formdata),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            successNotification('Successfully added a comment!');
            getComments(1, id);

        },
        error: function (res) {
            dangerNotification('Something went wrong!');
        }
    });
})

function getComments(page, itemId) {
    let data = {
        'page': page,
        'itemId': itemId
    };

    $.ajax({
        type: 'GET',
        url: '/api/Comment/GetComments',
        data: data,
        dataType: 'json',
        contentType: 'application/json',
        success: function (res) {
            setComments(res);
            setPagination(res);
        },
        error: function (res) {
            dangerNotification('Something went wrong!');
        }
    });
}

function setComments(commentsObj) {
    let commentDivs = document.querySelectorAll('.comments .comment');
    commentDivs.forEach(x => x.remove());
    let comments = commentsObj.comments;
    let commentsToAdd = comments.reduce((acc, curr) => {
        let html = `<div class="col-md-5 comment" id=${curr.id}>
        <div class="comment-likes">
            <i style="font-size:36px;" class="fa like">&#xf087;</i>
            <span class="like-count">${curr.likesCount}</span>
            <i style="font-size:36px" class="fa dislike">&#xf165;</i>
            <span class="dislike-count">${curr.dislikesCount}</span>
        </div>
        <span class="comment-content">${curr.commentText}</span>
        <span class="comment-right">by ${curr.authorName}</span>`;
        if (curr.isCommenter) {
            html += `<button type="button" class="btn delete">&#10060;</button>`;
        }
    html += `</div>`;
        return acc + html;
    }, "");

    let commentsDiv = document.querySelector('.comments');
    commentsDiv.innerHTML = commentsToAdd;
}

function setPagination(comments) {
    let ul = document.querySelector('.pagination');
    let html = "";

    // Previous button
    if (comments.hasPreviousPage) {
        html += `<li class="page-item">
                <a class="page-link" tabindex="-1">Previous</a>
            </li>`
    } else {
        html += `<li class="page-item disabled">
                <a class="page-link" tabindex="-1">Previous</a>
            </li>`
    }

    // Pre current page
    for (var i = comments.page - 4; i < comments.page; i++){
        if(i > 0) {
        html += `<li class="page-item">
                        <a class="page-link" > ${i}</a>
                </li>`;
    }

}

// Current page

html += `<li class="page-item active">
        <a class="page-link" > ${comments.page}</a>
    </li>`;

// After current page

for (var i = comments.page + 1; i < comments.page + 4; i++) {
    if (i <= comments.pagesCount) {
        html += `<li class="page-item">
                        <a class="page-link" > ${i}</a>
                </li>`
    }

}

// Next page button

if (comments.hasNextPage) {
    html += `<li class="page-item">
                <a class="page-link">Next</a>
            </li>`;
} else {
    html += `<li class="page-item disabled">
                <a class="page-link">Next</a>
            </li>`;
}

ul.innerHTML = html;
        }

$('.pagination').on('click', '.page-link', function (e) {
    e.preventDefault()
    let pageNumber = e.target.innerHTML;

    if (!isNaN(pageNumber)) {
        getComments(pageNumber, id);
    } else if (pageNumber === "Previous") {
        let number = document.querySelector('.pagination .active a').innerHTML;
        getComments(Number(number) - 1, id);
    } else if (pageNumber === "Next") {
        let number = document.querySelector('.pagination .active a').innerHTML;
        getComments(Number(number) + 1, id);
    }
})

$('.comments').on('click', '.comment .like', function (e) {
    e.preventDefault()
    let ikesCountElement = e.target.parentElement.querySelector('.like-count');
    let id = e.target.parentElement.parentElement.id;
    let antiForgeryToken = document.querySelector('.antiforgery input[name="__RequestVerificationToken"]').value;

    $.ajax({
        type: 'POST',
        url: '/api/Comment/AddLike',
        data: JSON.stringify(id),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (res.includes('cannot')) {
                dangerNotification(res);
            } else if (res.includes('liked')) {
                successNotification(res);
                ikesCountElement.innerHTML = Number(ikesCountElement.innerHTML) + 1;
            }
            else if (res.includes('removed')) {
                ikesCountElement.innerHTML = Number(ikesCountElement.innerHTML) - 1;
                successNotification(res);
            }
        },
        error: function (res) {
            dangerNotification("An error occured, try again and make sure you are logged in!");
        }
    });
})

$('.comments').on('click', '.comment .dislike', function (e) {
    e.preventDefault()
    let disLikesCountElement = e.target.parentElement.querySelector('.dislike-count');
    let id = e.target.parentElement.parentElement.id;
    let antiForgeryToken = document.querySelector('.antiforgery input[name="__RequestVerificationToken"]').value;

    $.ajax({
        type: 'POST',
        url: '/api/Comment/AddDislike',
        data: JSON.stringify(id),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (res.includes('cannot')) {
                dangerNotification(res);
            } else if (res.includes('disliked')) {
                successNotification(res);
                disLikesCountElement.innerHTML = Number(disLikesCountElement.innerHTML) + 1;
            }
            else if (res.includes('removed')) {
                disLikesCountElement.innerHTML = Number(disLikesCountElement.innerHTML) - 1;
                successNotification(res);
            }
        },
        error: function (res) {
            dangerNotification("An error occured, try again and make sure you are logged in!");
        }
    });
})

$('.comments').on('click', '.comment .delete', function (e) {
    e.preventDefault()
    let commentId = Number(e.target.parentElement.id);
    let antiForgeryToken = document.querySelector('.antiforgery input[name="__RequestVerificationToken"]').value;
    $.ajax({
        type: 'POST',
        url: '/api/Comment/DeleteComment',
        data: JSON.stringify(commentId),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            successNotification("You successfully deleted the comment!");
            let currentPage = document.querySelector('.pagination .active').innerText;
            getComments(currentPage, id);
        },
        error: function (res) {
            dangerNotification("Something went wrong, try again!");
            console.log(res);
        }
    });
})

// Get the starting comments
getComments(1, id);