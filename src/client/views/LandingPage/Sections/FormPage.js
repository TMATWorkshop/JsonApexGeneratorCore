import React from "react";
// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";
import TextField from '@material-ui/core/TextField';
import { useState } from 'react';

// @material-ui/icons
import CallMade from "@material-ui/icons/CallMade";
import CallReceived from "@material-ui/icons/CallReceived";
import Tune from "@material-ui/icons/Tune";
import NativeSelect from '@material-ui/core/NativeSelect';
import InputLabel from '@material-ui/core/InputLabel';

// core components
import Button from "../../../components/CustomButtons/Button.js";
import GridContainer from "../../../components/Grid/GridContainer.js";
import GridItem from "../../../components/Grid/GridItem.js";
import InfoArea from "../../../components/InfoArea/InfoArea.js";

import api from '../../../api';

import productStyle from "../../../assets/jss/material-kit-react/views/landingPageSections/productStyle.js";

class FormPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {className: '', 
                  classNameMsg: 'Enter Class Name prefix',
                  classNameError: false,
                  urlExtension: '', 
                  urlExtensionMsg: 'Enter URL Extension after Named credential',
                  urlExtensionError: false,
                  namedCredential: '', 
                  namedCredentialMsg: 'Enter Named Credential API Name',
                  namedCredentialError: false,
                  requestType: 'POST',
                  requestJSON: '',
                  requestJSONErrorMsg: '',
                  requestJSONError: false,
                  responseJSON: '',
                  responseJSONErrorMsg: '',
                  responseJSONError: false,
                  requestJSONValid: false,
                  responseJSONValid: false
                };

    this.handleChange = this.handleChange.bind(this);
    this.validateRequestJSON = this.validateRequestJSON.bind(this);
    this.validateResponseJSON = this.validateResponseJSON.bind(this);
    this.generateFiles = this.generateFiles.bind(this);
  }

  handleChange(e) {
    let change = {};
    change[e.target.name] = e.target.value;
    this.setState(change);
  }

  validateRequestJSON(event) {
    try {
      var jsonObj = JSON.parse(this.state.requestJSON);
      var prettyJSON = JSON.stringify(jsonObj, undefined, 2);
      this.setState({requestJSON: prettyJSON }); 
      this.setState({ requestJSONErrorMsg : '', requestJSONError : false });
      this.setState({ requestJSONValid : true });
    }
    catch (err) {
      console.log(err);
      this.setState({ requestJSONErrorMsg : err.message, requestJSONError : true });
      this.setState({ requestJSONValid : false });
    }
  }

  validateResponseJSON(event) {
    try {
      var jsonObj = JSON.parse(this.state.responseJSON);
      var prettyJSON = JSON.stringify(jsonObj, undefined, 2);
      this.setState({responseJSON: prettyJSON });
      this.setState({ responseJSONErrorMsg : '', responseJSONError : false });
      this.setState({ responseJSONValid : true });
    }
    catch (err) {
      //Highlight Error in JSON file
      console.log(err.message);
      this.setState({ responseJSONErrorMsg : err.message, responseJSONError : true });
      this.setState({ responseJSONValid : false });
    }
  }

  generateFiles(event) {
    this.validateResponseJSON(event);
    this.validateRequestJSON(event);
    if (this.state.responseJSONValid && 
        this.state.requestJSONValid && 
        this.state.className.length > 0 &&
        this.state.namedCredential.length > 0 &&
        this.state.urlExtension.length > 0 &&
        this.state.requestType.length > 0 &&
        this.state.requestJSON.length > 0 &&
        this.state.responseJSON.length > 0) {
      api.generateFiles(this.state.className, this.state.namedCredential, this.state.urlExtension, this.state.requestType, this.state.requestJSON, this.state.responseJSON);
    }
    else {
      alert('Please check your input');
    }
  }

  render() {
    const { classes } = this.props;
    return (
      <div className={classes.section}>
        <div>
          <GridContainer>
            <GridItem xs={12} sm={12} md={12}>
              <InfoArea
                title="Properties"
                description=""
                icon={Tune}
                iconColor="danger"
                vertical
              />
              <GridContainer>
                <GridItem xs={12} sm={6} md={6}>
                  <TextField
                    id="txtClassName"
                    name="className"
                    label={this.state.classNameMsg}
                    variant="outlined"
                    fullWidth
                    value={this.state.className}
                    onChange={this.handleChange}
                    required
                    error={this.state.classNameError}
                  />
                </GridItem>
                <GridItem xs={12} sm={6} md={6}>
                  <InputLabel htmlFor="ddlRequestType">Request Type</InputLabel>
                  <NativeSelect
                    id="ddlRequestType"
                    fullWidth
                    variant="outlined"
                    value={this.state.requestType}
                    onChange={this.handleChange}
                    required
                  >
                    <option value="POST">POST</option>
                  </NativeSelect>
                </GridItem>
                <GridItem xs={12} sm={6} md={6}>
                  <TextField
                    id="txtNamedCredential"
                    name="namedCredential"
                    label={this.state.namedCredentialMsg}
                    variant="outlined"
                    fullWidth
                    value={this.state.namedCredential}
                    onChange={this.handleChange}
                    required
                    error={this.state.namedCredentialError}
                />
                </GridItem>
                <GridItem xs={12} sm={6} md={6}>
                  <TextField
                    id="txtURLExtension"
                    name="urlExtension"
                    label={this.state.urlExtensionMsg}
                    variant="outlined"
                    fullWidth
                    value={this.state.urlExtension}
                    onChange={this.handleChange}
                    required
                    error={this.state.urlExtensionError}
                />
                </GridItem>
                <GridItem xs={12}>
                  <Button 
                    variant="contained"
                    color="primary"
                    onClick={this.generateFiles}>
                    Generate Apex Files
                  </Button>
                </GridItem>
              </GridContainer>
            </GridItem>
          </GridContainer>
          <GridContainer>
            <GridItem xs={12} sm={12} md={6}>
              <InfoArea
                title="Request JSON"
                description=""
                icon={CallMade}
                iconColor="success"
                vertical
              />
              <TextField
                id="txtRequestJSON"
                name="requestJSON"
                style={{ margin: 8 }}
                placeholder="Paste your JSON string here"
                fullWidth
                margin="normal"
                multiline
                rows={50}
                variant="outlined"
                value={this.state.requestJSON}
                onChange={this.handleChange}
                helperText={this.state.requestJSONErrorMsg}
                error={this.state.requestJSONError}
                required
              />
                <Button variant="contained" color="primary" onClick={this.validateRequestJSON}>
                    Validate JSON
                  </Button>
            </GridItem>
            <GridItem xs={12} sm={12} md={6}>
              <InfoArea
                title="Response JSON"
                description=""
                icon={CallReceived}
                iconColor="success"
                vertical
              />
              <TextField
                id="txtResponseJSON"
                name="responseJSON"
                style={{ margin: 8 }}
                placeholder="Paste your JSON string here"
                fullWidth
                margin="normal"
                multiline
                rows={50}
                variant="outlined"
                value={this.state.responseJSON}
                onChange={this.handleChange}
                helperText={this.state.responseJSONErrorMsg}
                error={this.state.responseJSONError}
                required
              />
              <Button variant="contained" color="primary" onClick={this.validateResponseJSON}>
                    Validate JSON
                  </Button>
            </GridItem>
          </GridContainer>
        </div>
        <GridContainer justify="center">
          <GridItem xs={12} sm={12} md={8}>
            <h3 className={classes.title}>Usage</h3>
            <h5 className={classes.description}>
              <ol>
                <li>Enter a value for all the fields</li>
                <li>Click on "Generate Apex Files"</li>
                <li>Extract the zip file and copy the contents to the classes folder of your SFDX project or copy the contents of the .cls files to your Developer Console</li>                
              </ol>
            </h5>
          </GridItem>
        </GridContainer>
      </div>
    );
  }
}

export default withStyles(productStyle)(FormPage);
