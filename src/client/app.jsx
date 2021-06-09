import React, {Component} from 'react';
// nodejs library that concatenates classes
import classNames from "classnames";
// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";

// core components
import Header from "./components/Header/Header.js";
import Parallax from "./components/Parallax/Parallax.js";

import landingPageStyle from "./assets/jss/material-kit-react/views/components.js";

// Sections for this page
import FormPage from "./views/LandingPage/Sections/FormPage.js"
import api from './api';

const dashboardRoutes = [];
//export default class List extends Component {
class App extends Component {
	constructor(props) {
		super(props);

		this.state = {
			items: []
		};
	}

	componentDidMount() {
	}

	render() {
		const {items} = this.state;
		const { classes, ...rest } = this.props;
		/*
		return (
			<div>
				<h1>Values</h1>
				{
					items.map((item, index) => (
						<li key={index} className="list-group-item">
							{item}
						</li>
					))
				}
			</div>
		);
		*/
		return (
			<div>
			  <Header
				color="transparent"
				routes={dashboardRoutes}
				brand="JSON to Apex Code Generator"
				fixed
				changeColorOnScroll={{
				  height: 0,
				  color: "white"
				}}
				{...rest}
			  />
			  <Parallax filter image={require("./assets/img/landing-bg.jpg")}>
				
			  </Parallax>
			  	<div className={classNames(classes.main, classes.mainRaised)}>
				<div className={classes.container}>
				  <FormPage />
				</div>
			  </div>
			  
			</div>
		  );
	}
}

export default withStyles(landingPageStyle)(App);