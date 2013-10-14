from flask import render_template, request
from app import app

@app.route('/')
@app.route('/index', methods=['GET'])
def index():
    amount = request.args.get('amount')
    return render_template("mainpage.html", amount=amount)
